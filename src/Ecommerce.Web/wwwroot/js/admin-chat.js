// Admin Chat Dashboard JavaScript with SignalR
(function () {
    'use strict';

    // ===== Variables =====
    let connection = null;
    let currentSessionToken = null;
    let isOnline = true;
    let sessions = [];

    // DOM Elements
    const toggleOnlineBtn = document.getElementById('toggle-online-btn');
    const sessionsListEl = document.getElementById('chat-sessions-list');
    const sessionsCountEl = document.getElementById('sessions-count');
    const chatMessagesEl = document.getElementById('admin-chat-messages');
    const chatInput = document.getElementById('admin-chat-input');
    const sendBtn = document.getElementById('admin-send-btn');
    const currentCustomerName = document.getElementById('current-customer-name');
    const inputContainer = document.getElementById('admin-input-container');
    const closeSessionBtn = document.getElementById('close-session-btn');

    // ===== Initialize =====
    function init() {
        setupSignalR();
        setupEventListeners();
        loadActiveSessions();
    }

    // ===== SignalR Setup =====
    function setupSignalR() {
        connection = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .withAutomaticReconnect()
            .build();

        // Receive new session
        connection.on('ReceiveNewSession', (sessionToken, customerName, message) => {
            console.log('New session:', sessionToken, customerName);
            loadActiveSessions();
            showNotification(`Cuộc trò chuyện mới từ ${customerName}`);
        });

        // Receive customer message
        connection.on('ReceiveCustomerMessage', (sessionToken, message, customerName, timestamp) => {
            console.log('Customer message:', sessionToken, message);

            // Update sessions list
            loadActiveSessions();

            // If this is current session, display message
            if (currentSessionToken === sessionToken) {
                addMessageToChat(message, customerName, 'customer', timestamp);
            } else {
                showNotification(`Tin nhắn mới từ ${customerName}`);
            }
        });

        // Message sent confirmation
        connection.on('MessageSent', (sessionToken, message, timestamp) => {
            if (currentSessionToken === sessionToken) {
                addMessageToChat(message, 'Bạn', 'admin', timestamp);
            }
        });

        // Error
        connection.on('Error', (errorMessage) => {
            showError(errorMessage);
        });

        // Connection events
        connection.onreconnecting(() => {
            console.log('SignalR reconnecting...');
        });

        connection.onreconnected(() => {
            console.log('SignalR reconnected');
            connection.invoke('JoinAdminHub');
            connection.invoke('SetAdminOnline', isOnline);
        });

        connection.onclose(() => {
            console.log('SignalR disconnected');
        });

        // Start connection
        startConnection();
    }

    async function startConnection() {
        try {
            await connection.start();
            console.log('SignalR connected');
            await connection.invoke('JoinAdminHub');
            await connection.invoke('SetAdminOnline', isOnline);
        } catch (err) {
            console.error('SignalR connection error:', err);
            setTimeout(startConnection, 5000);
        }
    }

    // ===== Event Listeners =====
    function setupEventListeners() {
        // Toggle online status
        toggleOnlineBtn.addEventListener('click', async () => {
            isOnline = !isOnline;

            if (isOnline) {
                toggleOnlineBtn.className = 'btn btn-success';
                toggleOnlineBtn.innerHTML = '<i class="bi bi-circle-fill"></i> Đang Online';
            } else {
                toggleOnlineBtn.className = 'btn btn-secondary';
                toggleOnlineBtn.innerHTML = '<i class="bi bi-circle"></i> Offline';
            }

            await connection.invoke('SetAdminOnline', isOnline);
        });

        // Send message
        sendBtn.addEventListener('click', () => {
            sendMessage();
        });

        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });

        // Close session
        closeSessionBtn.addEventListener('click', async () => {
            if (!currentSessionToken) return;

            if (confirm('Bạn có chắc muốn đóng cuộc trò chuyện này?')) {
                const session = sessions.find(s => s.SessionToken === currentSessionToken);
                if (session) {
                    try {
                        const response = await fetch(`/Admin/Chat/CloseSession?sessionId=${session.Id}`, {
                            method: 'POST'
                        });

                        if (response.ok) {
                            showSuccess('Đã đóng cuộc trò chuyện');
                            currentSessionToken = null;
                            loadActiveSessions();
                            clearChatArea();
                        }
                    } catch (err) {
                        console.error('Error closing session:', err);
                        showError('Không thể đóng cuộc trò chuyện');
                    }
                }
            }
        });

        // Auto-refresh sessions every 30 seconds
        setInterval(() => {
            loadActiveSessions();
        }, 30000);
    }

    // ===== Sessions Management =====
    async function loadActiveSessions() {
        try {
            const response = await fetch('/Admin/Chat/GetActiveSessions');
            sessions = await response.json();

            renderSessionsList();
            sessionsCountEl.textContent = sessions.length;
        } catch (err) {
            console.error('Error loading sessions:', err);
        }
    }

    function renderSessionsList() {
        if (sessions.length === 0) {
            sessionsListEl.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="bi bi-inbox" style="font-size: 48px;"></i>
                    <p class="mt-2">Chưa có cuộc trò chuyện nào</p>
                </div>
            `;
            return;
        }

        sessionsListEl.innerHTML = sessions.map(session => `
            <div class="list-group-item chat-session-item ${session.SessionToken === currentSessionToken ? 'active' : ''}" 
                 data-session-token="${session.SessionToken}">
                <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                        <h6 class="mb-1">
                            <i class="bi ${session.IsAiHandling ? 'bi-robot' : 'bi-person'}"></i>
                            ${session.CustomerName || 'Khách hàng'}
                        </h6>
                        <p class="mb-1 text-muted small text-truncate" style="max-width: 200px;">
                            ${session.LastMessage || 'Chưa có tin nhắn'}
                        </p>
                        <small class="text-muted">${formatRelativeTime(session.LastMessageAt)}</small>
                    </div>
                    <div>
                        ${session.UnreadCount > 0 ? `<span class="unread-badge">${session.UnreadCount}</span>` : ''}
                    </div>
                </div>
            </div>
        `).join('');

        // Add click handlers
        document.querySelectorAll('.chat-session-item').forEach(item => {
            item.addEventListener('click', () => {
                selectSession(item.dataset.sessionToken);
            });
        });
    }

    async function selectSession(sessionToken) {
        currentSessionToken = sessionToken;

        try {
            const response = await fetch(`/Admin/Chat/GetSessionHistory?sessionToken=${sessionToken}`);
            const data = await response.json();

            currentCustomerName.textContent = data.session.CustomerName || 'Khách hàng';
            inputContainer.style.display = 'block';
            closeSessionBtn.style.display = 'inline-block';

            // Render messages
            chatMessagesEl.innerHTML = '';
            data.messages.forEach(msg => {
                addMessageToChat(msg.Message, msg.SenderName, msg.SenderType, msg.SentAt);
            });

            // Re-render sessions to update active state
            renderSessionsList();

            chatInput.focus();
        } catch (err) {
            console.error('Error loading session history:', err);
            showError('Không thể tải lịch sử chat');
        }
    }

    function clearChatArea() {
        currentCustomerName.textContent = 'Chọn cuộc trò chuyện';
        chatMessagesEl.innerHTML = `
            <div class="text-center text-muted py-5">
                <i class="bi bi-chat-left-text" style="font-size: 64px; opacity: 0.3;"></i>
                <p class="mt-3">Chọn một cuộc trò chuyện để bắt đầu</p>
            </div>
        `;
        inputContainer.style.display = 'none';
        closeSessionBtn.style.display = 'none';
    }

    // ===== Messaging =====
    async function sendMessage() {
        const message = chatInput.value.trim();
        if (!message || !currentSessionToken) return;

        chatInput.value = '';
        sendBtn.disabled = true;

        try {
            await connection.invoke('SendMessageFromAdmin', currentSessionToken, message);
            sendBtn.disabled = false;
            chatInput.focus();
        } catch (err) {
            console.error('Error sending message:', err);
            showError('Không thể gửi tin nhắn');
            sendBtn.disabled = false;
        }
    }

    function addMessageToChat(message, senderName, senderType, timestamp) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `chat-message ${senderType}`;

        const bubbleDiv = document.createElement('div');
        bubbleDiv.className = 'message-bubble';

        const messageText = document.createElement('div');
        messageText.textContent = message;
        bubbleDiv.appendChild(messageText);

        const timeDiv = document.createElement('div');
        timeDiv.className = 'message-time';
        timeDiv.textContent = formatTime(timestamp);
        bubbleDiv.appendChild(timeDiv);

        messageDiv.appendChild(bubbleDiv);
        chatMessagesEl.appendChild(messageDiv);

        chatMessagesEl.scrollTop = chatMessagesEl.scrollHeight;
    }

    // ===== Utilities =====
    function formatTime(timestamp) {
        const date = timestamp ? new Date(timestamp) : new Date();
        return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
    }

    function formatRelativeTime(timestamp) {
        const now = new Date();
        const date = new Date(timestamp);
        const diffMs = now - date;
        const diffMins = Math.floor(diffMs / 60000);

        if (diffMins < 1) return 'Vừa xong';
        if (diffMins < 60) return `${diffMins} phút trước`;

        const diffHours = Math.floor(diffMins / 60);
        if (diffHours < 24) return `${diffHours} giờ trước`;

        const diffDays = Math.floor(diffHours / 24);
        return `${diffDays} ngày trước`;
    }

    function showNotification(message) {
        // Could use toast notifications here
        console.log('Notification:', message);
    }

    function showError(message) {
        alert('Lỗi: ' + message);
    }

    function showSuccess(message) {
        alert(message);
    }

    // ===== Initialize =====
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

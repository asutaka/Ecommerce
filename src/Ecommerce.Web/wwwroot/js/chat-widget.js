// Chat Widget JavaScript with SignalR
(function () {
    'use strict';

    // ===== Variables =====
    let connection = null;
    let sessionToken = null;
    let customerName = null;
    let isConnected = false;

    // DOM Elements
    const widget = document.getElementById('chat-widget');
    const toggleBtn = document.getElementById('chat-toggle-btn');
    const chatWindow = document.getElementById('chat-window');
    const minimizeBtn = document.getElementById('chat-minimize-btn');
    const closeBtn = document.getElementById('chat-close-btn');
    const messagesContainer = document.getElementById('chat-messages');
    const chatInput = document.getElementById('chat-input');
    const sendBtn = document.getElementById('chat-send-btn');
    const typingIndicator = document.getElementById('chat-typing-indicator');
    const unreadBadge = document.getElementById('chat-unread-badge');
    const chatStatus = document.getElementById('chat-status');

    let unreadCount = 0;
    let isWindowOpen = false;

    // ===== Initialize =====
    function init() {
        // Get or create session token
        sessionToken = localStorage.getItem('chat_session_token');
        if (!sessionToken) {
            sessionToken = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            localStorage.setItem('chat_session_token', sessionToken);
        }

        // Get customer name if logged in
        customerName = document.getElementById('customer-name-hidden')?.value || null;

        // Setup SignalR
        setupSignalR();

        // Setup event listeners
        setupEventListeners();
    }

    // ===== SignalR Setup =====
    function setupSignalR() {
        connection = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .withAutomaticReconnect()
            .build();

        // Receive message from admin/AI
        connection.on('ReceiveMessage', (message, senderName, timestamp) => {
            addMessage(message, senderName, false, timestamp);
            showTypingIndicator(false);

            if (!isWindowOpen) {
                incrementUnreadCount();
            }
        });

        // Connection events
        connection.onreconnecting(() => {
            chatStatus.textContent = 'Đang kết nối lại...';
            console.log('SignalR reconnecting...');
        });

        connection.onreconnected(() => {
            chatStatus.textContent = 'Đã kết nối';
            console.log('SignalR reconnected');
            joinChat();
        });

        connection.onclose(() => {
            isConnected = false;
            chatStatus.textContent = 'Mất kết nối';
            console.log('SignalR disconnected');
        });

        // Start connection
        startConnection();
    }

    async function startConnection() {
        try {
            await connection.start();
            console.log('SignalR connected');
            isConnected = true;
            chatStatus.textContent = 'Trực tuyến';
            await joinChat();
        } catch (err) {
            console.error('SignalR connection error:', err);
            chatStatus.textContent = 'Lỗi kết nối';
            setTimeout(startConnection, 5000);
        }
    }

    async function joinChat() {
        if (!isConnected) return;

        try {
            await connection.invoke('JoinChat', sessionToken);
            console.log('Joined chat session:', sessionToken);
        } catch (err) {
            console.error('Error joining chat:', err);
        }
    }

    // ===== Event Listeners =====
    function setupEventListeners() {
        // Toggle chat window
        toggleBtn.addEventListener('click', () => {
            toggleChatWindow();
        });

        // Minimize button
        minimizeBtn.addEventListener('click', () => {
            closeChatWindow();
        });

        // Close button
        closeBtn.addEventListener('click', () => {
            closeChatWindow();
        });

        // Send message on button click
        sendBtn.addEventListener('click', () => {
            sendMessage();
        });

        // Send message on Enter key
        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });

        // Auto-resize input
        chatInput.addEventListener('input', () => {
            sendBtn.disabled = chatInput.value.trim() === '';
        });
    }

    // ===== UI Functions =====
    function toggleChatWindow() {
        if (isWindowOpen) {
            closeChatWindow();
        } else {
            openChatWindow();
        }
    }

    function openChatWindow() {
        chatWindow.style.display = 'flex';
        isWindowOpen = true;
        unreadCount = 0;
        updateUnreadBadge();
        chatInput.focus();

        // Remove welcome message if exists
        const welcomeMsg = messagesContainer.querySelector('.chat-welcome-message');
        if (welcomeMsg && messagesContainer.querySelectorAll('.chat-message').length > 0) {
            welcomeMsg.style.display = 'none';
        }
    }

    function closeChatWindow() {
        chatWindow.style.display = 'none';
        isWindowOpen = false;
    }

    function incrementUnreadCount() {
        unreadCount++;
        updateUnreadBadge();
    }

    function updateUnreadBadge() {
        if (unreadCount > 0 && !isWindowOpen) {
            unreadBadge.textContent = unreadCount > 9 ? '9+' : unreadCount;
            unreadBadge.style.display = 'flex';
        } else {
            unreadBadge.style.display = 'none';
        }
    }

    function showTypingIndicator(show) {
        typingIndicator.style.display = show ? 'flex' : 'none';
        if (show) {
            scrollToBottom();
        }
    }

    function scrollToBottom() {
        setTimeout(() => {
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        }, 100);
    }

    // ===== Message Functions =====
    async function sendMessage() {
        const message = chatInput.value.trim();
        if (!message || !isConnected) return;

        // Add to UI immediately
        addMessage(message, 'Bạn', true);

        // Clear input
        chatInput.value = '';
        sendBtn.disabled = true;

        // Show typing indicator
        showTypingIndicator(true);

        // Send via SignalR
        try {
            await connection.invoke('SendMessageFromCustomer', sessionToken, message, customerName || 'Khách hàng');
        } catch (err) {
            console.error('Error sending message:', err);
            showTypingIndicator(false);
            showError('Không thể gửi tin nhắn. Vui lòng thử lại.');
        }
    }

    function addMessage(message, senderName, isSent, timestamp = null) {
        // Remove welcome message if exists
        const welcomeMsg = messagesContainer.querySelector('.chat-welcome-message');
        if (welcomeMsg) {
            welcomeMsg.style.display = 'none';
        }

        const messageDiv = document.createElement('div');
        messageDiv.className = `chat-message ${isSent ? 'sent' : 'received'}`;

        const bubbleDiv = document.createElement('div');
        bubbleDiv.className = 'message-bubble';

        if (!isSent && senderName) {
            const senderSpan = document.createElement('div');
            senderSpan.className = 'message-sender';
            senderSpan.textContent = senderName;
            bubbleDiv.appendChild(senderSpan);
        }

        const messageText = document.createElement('div');
        messageText.textContent = message;
        bubbleDiv.appendChild(messageText);

        const timeSpan = document.createElement('div');
        timeSpan.className = 'message-time';
        timeSpan.textContent = formatTime(timestamp);
        bubbleDiv.appendChild(timeSpan);

        messageDiv.appendChild(bubbleDiv);
        messagesContainer.appendChild(messageDiv);

        scrollToBottom();
    }

    function formatTime(timestamp) {
        const date = timestamp ? new Date(timestamp) : new Date();
        return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
    }

    function showError(errorMessage) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'chat-error-message';
        errorDiv.textContent = errorMessage;
        messagesContainer.appendChild(errorDiv);

        setTimeout(() => {
            errorDiv.remove();
        }, 5000);

        scrollToBottom();
    }

    // ===== Initialize on DOM ready =====
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

// Cart functionality
const cartBadge = document.getElementById('cart-badge-count');

// Update cart badge count
function updateCartBadge(count) {
    if (cartBadge) {
        cartBadge.textContent = count;
        cartBadge.style.display = 'flex';
    }
}

// Add to cart (called from product pages)
async function addToCart(productId, quantity = 1, variantId = null) {
    try {
        let body = `productId=${productId}&quantity=${quantity}`;
        if (variantId && variantId.trim() !== '') {
            body += `&variantId=${variantId}`;
        }

        const response = await fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: body
        });

        const result = await response.json();

        if (result.success) {
            updateCartBadge(result.itemCount);
            showNotification(result.message, 'success');
        } else {
            showNotification(result.message, 'error');
        }
    } catch (error) {
        console.error('Error adding to cart:', error);
        showNotification('Có lỗi xảy ra', 'error');
    }
}

// Buy Now function
async function buyNow(productId, quantity = 1, variantId = null) {
    try {
        let body = `productId=${productId}&quantity=${quantity}`;
        if (variantId && variantId.trim() !== '') {
            body += `&variantId=${variantId}`;
        }

        const response = await fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: body
        });

        const result = await response.json();

        if (result.success) {
            window.location.href = '/Cart';
        } else {
            showNotification(result.message, 'error');
        }
    } catch (error) {
        console.error('Error buying now:', error);
        showNotification('Có lỗi xảy ra', 'error');
    }
}

// Update quantity (cart page)
async function updateQuantity(cartItemId, quantity) {
    if (quantity < 0) return;

    try {
        const response = await fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `cartItemId=${cartItemId}&quantity=${quantity}`
        });

        const result = await response.json();

        if (result.success) {
            if (quantity === 0) {
                // Remove item from DOM
                const itemElement = document.querySelector(`[data-item-id="${cartItemId}"]`);
                if (itemElement) {
                    itemElement.remove();
                }
            } else {
                // Update quantity display
                const itemElement = document.querySelector(`[data-item-id="${cartItemId}"]`);
                if (itemElement) {
                    const quantitySpan = itemElement.querySelector('.quantity');
                    if (quantitySpan) {
                        quantitySpan.textContent = quantity;
                    }
                }
            }

            // Update totals
            updateCartTotals(result.subtotal, result.total);
            updateCartBadge(result.itemCount);

            // Check if cart is empty
            if (result.itemCount === 0) {
                location.reload();
            }
        } else {
            showNotification(result.message, 'error');
        }
    } catch (error) {
        console.error('Error updating quantity:', error);
        showNotification('Có lỗi xảy ra', 'error');
    }
}

// Remove item (cart page)
async function removeItem(cartItemId) {
    if (!confirm('Bạn có chắc muốn xóa sản phẩm này?')) {
        return;
    }

    try {
        const response = await fetch('/Cart/RemoveItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `cartItemId=${cartItemId}`
        });

        const result = await response.json();

        if (result.success) {
            // Remove item from DOM
            const itemElement = document.querySelector(`[data-item-id="${cartItemId}"]`);
            if (itemElement) {
                itemElement.remove();
            }

            // Update totals
            updateCartTotals(result.subtotal, result.total);
            updateCartBadge(result.itemCount);
            showNotification(result.message, 'success');

            // Check if cart is empty
            if (result.itemCount === 0) {
                location.reload();
            }
        } else {
            showNotification(result.message, 'error');
        }
    } catch (error) {
        console.error('Error removing item:', error);
        showNotification('Có lỗi xảy ra', 'error');
    }
}

// Update cart totals display
function updateCartTotals(subtotal, total) {
    const subtotalElement = document.getElementById('subtotal');
    const totalElement = document.getElementById('total');

    if (subtotalElement) {
        subtotalElement.textContent = formatCurrency(subtotal);
    }
    if (totalElement) {
        totalElement.textContent = formatCurrency(total);
    }
}

// Format currency (Vietnamese Dong)
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0
    }).format(amount);
}

// Show notification toast
function showNotification(message, type = 'info') {
    // Remove existing notifications
    const existing = document.querySelector('.toast-notification');
    if (existing) {
        existing.remove();
    }

    // Create notification element
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

    // Show with animation
    setTimeout(() => toast.classList.add('show'), 10);

    // Auto-hide after 3 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Initialize cart badge on page load
document.addEventListener('DOMContentLoaded', async () => {
    try {
        const response = await fetch('/Cart/GetItemCount');
        const result = await response.json();
        updateCartBadge(result.itemCount);
    } catch (error) {
        console.error('Error loading cart count:', error);
    }
});

// Apply coupon code
async function applyCoupon() {
    const couponInput = document.getElementById('couponCodeInput');
    const couponCode = couponInput?.value?.trim();

    if (!couponCode) {
        showNotification('Vui lòng nhập mã giảm giá', 'error');
        return;
    }

    try {
        const response = await fetch('/Cart/ApplyCoupon', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `couponCode=${encodeURIComponent(couponCode)}`
        });

        const result = await response.json();

        if (result.success) {
            // Save form data before reload
            saveFormData();

            showNotification(result.message, 'success');
            // Reload page to show applied coupon
            setTimeout(() => location.reload(), 1000);
        } else {
            showNotification(result.message, 'error');
        }
    } catch (error) {
        console.error('Error applying coupon:', error);
        showNotification('Có lỗi xảy ra', 'error');
    }
}

// Save form data to sessionStorage
function saveFormData() {
    const formData = {
        customerName: document.querySelector('input[name="CustomerName"]')?.value || '',
        customerPhone: document.querySelector('input[name="CustomerPhone"]')?.value || '',
        customerEmail: document.querySelector('input[name="CustomerEmail"]')?.value || '',
        shippingAddress: document.querySelector('select[name="ShippingAddress"]')?.value ||
            document.querySelector('input[name="ShippingAddress"]')?.value || '',
        customAddress: document.getElementById('customAddress')?.value || '',
        note: document.querySelector('input[name="Note"]')?.value || '',
        paymentMethod: document.querySelector('input[name="PaymentMethod"]:checked')?.value || 'COD'
    };
    sessionStorage.setItem('cartFormData', JSON.stringify(formData));
}

// Restore form data from sessionStorage
function restoreFormData() {
    const savedData = sessionStorage.getItem('cartFormData');
    if (!savedData) return;

    try {
        const formData = JSON.parse(savedData);

        // Restore customer info
        if (formData.customerName) {
            const nameInput = document.querySelector('input[name="CustomerName"]');
            if (nameInput) nameInput.value = formData.customerName;
        }

        if (formData.customerPhone) {
            const phoneInput = document.querySelector('input[name="CustomerPhone"]');
            if (phoneInput) phoneInput.value = formData.customerPhone;
        }

        if (formData.customerEmail) {
            const emailInput = document.querySelector('input[name="CustomerEmail"]');
            if (emailInput) emailInput.value = formData.customerEmail;
        }

        // Restore shipping address
        if (formData.shippingAddress) {
            const addressSelect = document.querySelector('select[name="ShippingAddress"]');
            const addressInput = document.querySelector('input[name="ShippingAddress"]');

            if (addressSelect) {
                addressSelect.value = formData.shippingAddress;
                // Trigger change event to show custom address if needed
                addressSelect.dispatchEvent(new Event('change'));
            } else if (addressInput) {
                addressInput.value = formData.shippingAddress;
            }
        }

        if (formData.customAddress) {
            const customInput = document.getElementById('customAddress');
            if (customInput) customInput.value = formData.customAddress;
        }

        // Restore note
        if (formData.note) {
            const noteInput = document.querySelector('input[name="Note"]');
            if (noteInput) noteInput.value = formData.note;
        }

        // Restore payment method
        if (formData.paymentMethod) {
            const paymentRadio = document.querySelector(`input[name="PaymentMethod"][value="${formData.paymentMethod}"]`);
            if (paymentRadio) paymentRadio.checked = true;
        }

        // Clear saved data after restore
        sessionStorage.removeItem('cartFormData');
    } catch (error) {
        console.error('Error restoring form data:', error);
    }
}

// Restore form data on page load
document.addEventListener('DOMContentLoaded', function () {
    restoreFormData();
});


// Remove applied coupon
async function removeCoupon() {
    if (!confirm('Bạn có chắc muốn xóa mã giảm giá?')) {
        return;
    }

    try {
        const response = await fetch('/Cart/RemoveCoupon', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            }
        });

        const result = await response.json();

        if (result.success) {
            // Save form data before reload
            saveFormData();

            showNotification(result.message, 'success');
            // Reload page to update UI
            setTimeout(() => location.reload(), 1000);
        } else {
            showNotification(result.message, 'error');
        }
    } catch (error) {
        console.error('Error removing coupon:', error);
        showNotification('Có lỗi xảy ra', 'error');
    }
}

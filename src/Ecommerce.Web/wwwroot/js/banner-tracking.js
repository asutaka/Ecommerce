/**
 * Banner Analytics Tracking
 * Tracks banner views and clicks for analytics
 */

/**
 * Track a banner view (impression)
 * @param {string} bannerId - GUID of the banner
 */
function trackBannerView(bannerId) {
    if (!bannerId || bannerId === '00000000-0000-0000-0000-000000000000') {
        return; // Invalid banner ID
    }

    fetch('/api/BannerAnalytics/track-view', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ bannerId: bannerId })
    }).catch(err => {
        // Silently fail - don't interrupt user experience
        console.log('Analytics tracking error:', err);
    });
}

/**
 * Track a banner click
 * @param {string} bannerId - GUID of the banner
 */
function trackBannerClick(bannerId) {
    if (!bannerId || bannerId === '00000000-0000-0000-0000-000000000000') {
        return; // Invalid banner ID
    }

    fetch('/api/BannerAnalytics/track-click', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ bannerId: bannerId })
    }).catch(err => {
        // Silently fail - don't interrupt user experience
        console.log('Analytics tracking error:', err);
    });
}

/**
 * Track banner views when they become visible in viewport
 * Uses Intersection Observer for efficient tracking
 */
function initBannerViewTracking() {
    // Only track if IntersectionObserver is available
    if (typeof IntersectionObserver === 'undefined') {
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && !entry.target.dataset.tracked) {
                const bannerId = entry.target.dataset.bannerId;
                if (bannerId) {
                    trackBannerView(bannerId);
                    entry.target.dataset.tracked = 'true'; // Mark as tracked
                }
            }
        });
    }, {
        threshold: 0.5 // Banner must be at least 50% visible
    });

    // Observe all elements with data-banner-id attribute
    document.querySelectorAll('[data-banner-id]').forEach(el => {
        observer.observe(el);
    });
}

// Initialize tracking when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initBannerViewTracking);
} else {
    initBannerViewTracking();
}

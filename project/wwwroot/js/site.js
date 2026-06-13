// Active nav highlighting
(function() {
    document.querySelectorAll('.active-check').forEach(function(el) {
        var data = el.getAttribute('data-active').split(',');
        var controller = data[0];
        var action = data[1];
        var path = window.location.pathname.toLowerCase();
        if (path.includes('/' + controller.toLowerCase() + '/' + action.toLowerCase()) ||
            (path.endsWith('/' + controller.toLowerCase()) && action === 'Index')) {
            el.classList.add('active');
        }
    });
})();

// Theme switcher (3 themes: oled-dark, teal-dark, light)
(function() {
    var themes = ['oled-dark', 'teal-dark', 'light'];
    var html = document.documentElement;
    var saved = localStorage.getItem('mehkawan-theme');
    if (saved && themes.indexOf(saved) !== -1) {
        html.setAttribute('data-theme', saved);
    }
})();

window.setTheme = function(theme) {
    var themes = ['oled-dark', 'teal-dark', 'light'];
    if (themes.indexOf(theme) === -1) return;
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('mehkawan-theme', theme);
};

// Toast notification
function showToast(message) {
    var toast = document.getElementById('copyToast');
    if (!toast) return;
    toast.innerText = message;
    toast.style.opacity = '1';
    toast.style.transform = 'translateY(0)';
    setTimeout(function() {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(12px)';
    }, 1800);
}

// Copy friend code with fun messages
var copyCount = 0;

function copyFriendCode() {
    var el = document.getElementById('friendCode');
    var code = el.innerText;

    if (el.dataset.copying === 'true') return;

    navigator.clipboard.writeText(code);
    el.dataset.copying = 'true';
    copyCount++;

    var messages = [
        { text: 'Copied!', count: 1 },
        { text: 'Copied again?', count: 2 },
        { text: 'Okay...', count: 3 },
        { text: 'Really?', count: 4 },
        { text: 'Stop it.', count: 5 },
        { text: 'BRO.', count: 6 },
        { text: 'You dead.', count: 7 },
        { text: 'Clown behavior.', count: 8 },
        { text: 'Bro is cooked.', count: 9 },
        { text: 'Okay bestie.', count: 10 },
    ];

    var msg = { text: 'Please stop.' };
    for (var i = 0; i < messages.length; i++) {
        if (messages[i].count === copyCount) {
            msg = messages[i];
            break;
        }
    }

    el.innerText = msg.text;
    showToast(msg.text);

    setTimeout(function() {
        el.innerText = code;
        el.dataset.copying = 'false';
    }, 2000);
}

// Service Worker
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function() {
        navigator.serviceWorker.register('/sw.js');
    });
}

// Smooth like button animation with confetti-like burst
(function() {
    document.addEventListener('click', function(e) {
        var btn = e.target.closest('.post-action-btn');
        if (!btn) return;
        var icon = btn.querySelector('i');
        if (icon && (icon.classList.contains('bi-heart') || icon.classList.contains('bi-heart-fill'))) {
            icon.style.transition = 'transform 0.25s cubic-bezier(0.16, 1, 0.3, 1)';
            icon.style.transform = 'scale(1.5)';
            setTimeout(function() {
                icon.style.transform = 'scale(1)';
            }, 250);
        }
    });
})();

// Staggered fade-in for page content
// Staggered fade-in for page content disabled to avoid unwanted animation on load.
// (function() {
//     // Original animation code removed.
// })();

// Floating particles on landing page
(function() {
    var bg = document.querySelector('.landing-bg');
    if (!bg) return;
    for (var i = 0; i < 8; i++) {
        var dot = document.createElement('div');
        dot.style.cssText = 'position:absolute;width:4px;height:4px;border-radius:50%;background:var(--accent);opacity:0;' +
            'top:' + (10 + Math.random() * 80) + '%;left:' + (10 + Math.random() * 80) + '%;' +
            'animation:particleFade ' + (6 + Math.random() * 8) + 's ease-in-out ' + (Math.random() * 5) + 's infinite;';
        bg.appendChild(dot);
    }
    var style = document.createElement('style');
    style.textContent = '@keyframes particleFade{0%,100%{opacity:0;transform:translateY(0) scale(0.5)}25%{opacity:0.4}50%{opacity:0.1;transform:translateY(-30px) scale(1)}75%{opacity:0.3}100%{opacity:0;transform:translateY(-50px) scale(0.5)}}';
    document.head.appendChild(style);
})();

// Global data-confirm handler for forms
document.addEventListener('submit', function(e) {
    var form = e.target;
    if (form.tagName !== 'FORM') return;
    var msg = form.getAttribute('data-confirm');
    if (!msg) return;
    e.preventDefault();
    showConfirm(msg, function() { form.submit(); });
});

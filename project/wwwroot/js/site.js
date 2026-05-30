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

(function() {
    var themes = ['oled-dark', 'teal-dark', 'light'];
    var icons = ['bi-moon-stars', 'bi-palette', 'bi-sun'];
    var html = document.documentElement;
    var btn = document.getElementById('themeBtn');
    if (!btn) return;
    var saved = localStorage.getItem('mehkawan-theme');
    if (saved && themes.indexOf(saved) !== -1) {
        html.setAttribute('data-theme', saved);
    }
    function updateIcon() {
        var idx = themes.indexOf(html.getAttribute('data-theme') || 'oled-dark');
        btn.innerHTML = '<i class="bi ' + icons[idx] + '"></i>';
    }
    updateIcon();
    btn.addEventListener('click', function() {
        var current = html.getAttribute('data-theme') || 'oled-dark';
        var idx = themes.indexOf(current);
        var next = themes[(idx + 1) % themes.length];
        html.setAttribute('data-theme', next);
        localStorage.setItem('mehkawan-theme', next);
        updateIcon();
    });
})();

function showToast(message) {
    var toast = document.getElementById('copyToast');
    toast.innerText = message;
    toast.style.opacity = '1';
    toast.style.transform = 'translateY(0)';
    setTimeout(function() {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(20px)';
    }, 1800);
}

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

if ('serviceWorker' in navigator) {
    window.addEventListener('load', function() {
        navigator.serviceWorker.register('/sw.js');
    });
}

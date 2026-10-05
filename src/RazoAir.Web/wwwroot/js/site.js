(function () {
    var toggle = document.getElementById('theme-toggle');
    if (!toggle) return;

    function currentTheme() {
        var attr = document.documentElement.getAttribute('data-theme');
        if (attr === 'light' || attr === 'dark') return attr;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    toggle.addEventListener('click', function () {
        var next = currentTheme() === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-theme', next);
        localStorage.setItem('razoair-theme', next);
    });

    document.querySelectorAll('.btn-swap-route').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var form = btn.closest('form');
            if (!form) return;
            var fromSelect = form.querySelector('#from');
            var toSelect = form.querySelector('#to');
            if (!fromSelect || !toSelect) return;

            var tempVal = fromSelect.value;
            fromSelect.value = toSelect.value;
            toSelect.value = tempVal;

            fromSelect.dispatchEvent(new Event('change', { bubbles: true }));
            toSelect.dispatchEvent(new Event('change', { bubbles: true }));

            btn.classList.add('is-rotating');
            setTimeout(function () {
                btn.classList.remove('is-rotating');
            }, 300);
        });
    });
})();

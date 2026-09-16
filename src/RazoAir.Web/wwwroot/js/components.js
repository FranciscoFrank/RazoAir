// Progressive-enhancement UI components: custom Select and Calendar (date) pickers.
// The original <select>/<input type="date"> stays in the DOM and keeps driving the
// form (name/value/required/asp-for binding all still work) — it is only hidden
// visually once a matching custom control has been built next to it, so the page
// still works correctly if this script fails to load.
(function () {
    'use strict';

    var MONTH_NAMES = ['January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'];
    var MONTH_SHORT = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var WEEKDAYS = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su']; // Monday-first

    var openControl = null;

    function closeOpenControl() {
        if (!openControl) return;
        openControl.close();
        openControl = null;
    }

    document.addEventListener('click', function (e) {
        if (openControl && !openControl.root.contains(e.target)) {
            closeOpenControl();
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && openControl) {
            var trigger = openControl.trigger;
            closeOpenControl();
            trigger.focus();
        }
    });

    function setInvalid(root, invalid) {
        root.classList.toggle('ra-invalid', !!invalid);
    }

    function dispatchChange(el) {
        el.dispatchEvent(new Event('input', { bubbles: true }));
        el.dispatchEvent(new Event('change', { bubbles: true }));
    }

    var uid = 0;
    function nextId(prefix) { uid += 1; return prefix + '-' + uid; }

    // Give the custom trigger an accessible name made of the field's visible
    // <label> plus the live value span, so screen readers announce both
    // ("From, Kyiv (KBP)") the way they would for a native labelled <select>.
    function linkLabel(originalEl, trigger, valueSpan) {
        var label = originalEl.id && document.querySelector('label[for="' + originalEl.id + '"]');
        if (!label) return;
        if (!label.id) label.id = nextId('ra-label');
        if (!valueSpan.id) valueSpan.id = nextId('ra-value');
        trigger.setAttribute('aria-labelledby', label.id + ' ' + valueSpan.id);
    }

    // ---------------- Select ----------------

    function buildSelect(select) {
        var root = document.createElement('div');
        root.className = 'ra-control ra-select';
        root.setAttribute('data-state', 'closed');

        var trigger = document.createElement('button');
        trigger.type = 'button';
        trigger.className = 'ra-trigger';
        trigger.setAttribute('aria-haspopup', 'listbox');
        trigger.setAttribute('aria-expanded', 'false');

        var valueSpan = document.createElement('span');
        valueSpan.className = 'ra-trigger-value';

        var caret = document.createElement('span');
        caret.className = 'ra-caret';
        caret.setAttribute('aria-hidden', 'true');
        caret.innerHTML = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m6 9 6 6 6-6"/></svg>';

        trigger.appendChild(valueSpan);
        trigger.appendChild(caret);
        linkLabel(select, trigger, valueSpan);

        var panel = document.createElement('ul');
        panel.className = 'ra-panel ra-select-panel';
        panel.setAttribute('role', 'listbox');
        panel.tabIndex = -1;
        panel.hidden = true;

        var options = Array.prototype.map.call(select.options, function (opt, index) {
            var li = document.createElement('li');
            li.className = 'ra-option';
            li.setAttribute('role', 'option');
            li.setAttribute('data-index', String(index));
            li.id = (select.id || 'ra-select') + '-opt-' + index;
            li.textContent = opt.text;
            li.setAttribute('aria-selected', opt.selected ? 'true' : 'false');
            panel.appendChild(li);
            return li;
        });

        select.parentNode.insertBefore(root, select);
        root.appendChild(trigger);
        root.appendChild(panel);
        root.appendChild(select);
        select.classList.add('ra-native-hidden');

        // The native control is now display:none, so the browser would silently
        // skip it during constraint validation anyway — remove `required` and
        // enforce it ourselves (see the ra:validate listener below) so we get a
        // consistent, styled error instead of a native validation bubble.
        var isRequired = select.required;
        select.required = false;

        var activeIndex = select.selectedIndex;
        var typeaheadBuffer = '';
        var typeaheadTimer = null;

        function labelFor(index) {
            var opt = select.options[index];
            return opt ? opt.text : '';
        }

        function refreshValue() {
            var opt = select.options[select.selectedIndex];
            var isPlaceholder = !opt || opt.value === '';
            valueSpan.textContent = opt ? opt.text : '';
            valueSpan.classList.toggle('is-placeholder', isPlaceholder);
            options.forEach(function (li, i) {
                li.setAttribute('aria-selected', i === select.selectedIndex ? 'true' : 'false');
            });
            if (isRequired && opt && opt.value !== '') {
                setInvalid(root, false);
            }
        }

        function setActive(index) {
            if (index < 0 || index >= options.length) return;
            activeIndex = index;
            options.forEach(function (li, i) { li.classList.toggle('is-active', i === index); });
            panel.setAttribute('aria-activedescendant', options[index].id);
            options[index].scrollIntoView({ block: 'nearest' });
        }

        function open() {
            panel.hidden = false;
            root.setAttribute('data-state', 'open');
            trigger.setAttribute('aria-expanded', 'true');
            setActive(select.selectedIndex >= 0 ? select.selectedIndex : 0);
            openControl = { root: root, close: close, trigger: trigger };
        }

        function close() {
            panel.hidden = true;
            root.setAttribute('data-state', 'closed');
            trigger.setAttribute('aria-expanded', 'false');
        }

        function choose(index) {
            select.selectedIndex = index;
            dispatchChange(select);
            refreshValue();
            close();
        }

        trigger.addEventListener('click', function () {
            if (root.getAttribute('data-state') === 'open') {
                closeOpenControl();
            } else {
                closeOpenControl();
                open();
                panel.focus();
            }
        });

        trigger.addEventListener('keydown', function (e) {
            if (e.key === 'ArrowDown' || e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                if (root.getAttribute('data-state') !== 'open') {
                    closeOpenControl();
                    open();
                }
                panel.focus();
            }
        });

        panel.addEventListener('keydown', function (e) {
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                setActive(Math.min(activeIndex + 1, options.length - 1));
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                setActive(Math.max(activeIndex - 1, 0));
            } else if (e.key === 'Home') {
                e.preventDefault();
                setActive(0);
            } else if (e.key === 'End') {
                e.preventDefault();
                setActive(options.length - 1);
            } else if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                choose(activeIndex);
                trigger.focus();
            } else if (e.key.length === 1 && /[a-z0-9]/i.test(e.key)) {
                typeaheadBuffer += e.key.toLowerCase();
                clearTimeout(typeaheadTimer);
                typeaheadTimer = setTimeout(function () { typeaheadBuffer = ''; }, 600);
                var match = options.findIndex(function (li) {
                    return li.textContent.toLowerCase().indexOf(typeaheadBuffer) === 0;
                });
                if (match >= 0) setActive(match);
            }
        });

        options.forEach(function (li, index) {
            li.addEventListener('click', function () {
                choose(index);
                trigger.focus();
            });
            li.addEventListener('mouseenter', function () { setActive(index); });
        });

        select.addEventListener('ra:validate', function (e) {
            if (isRequired && !select.value) {
                setInvalid(root, true);
                e.preventDefault();
                return;
            }
            setInvalid(root, false);
        });

        refreshValue();
        return root;
    }

    // ---------------- Calendar (date) ----------------

    function pad2(n) { return n < 10 ? '0' + n : String(n); }
    function toISO(d) { return d.getFullYear() + '-' + pad2(d.getMonth() + 1) + '-' + pad2(d.getDate()); }
    function parseISO(s) {
        if (!s) return null;
        var parts = s.split('-');
        if (parts.length !== 3) return null;
        return new Date(Number(parts[0]), Number(parts[1]) - 1, Number(parts[2]));
    }
    function sameDay(a, b) { return !!a && !!b && a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate(); }
    function formatLabel(d) { return d.getDate() + ' ' + MONTH_SHORT[d.getMonth()] + ' ' + d.getFullYear(); }

    function buildDate(input) {
        var root = document.createElement('div');
        root.className = 'ra-control ra-datepicker';
        root.setAttribute('data-state', 'closed');

        var trigger = document.createElement('button');
        trigger.type = 'button';
        trigger.className = 'ra-trigger';
        trigger.setAttribute('aria-haspopup', 'dialog');
        trigger.setAttribute('aria-expanded', 'false');

        var valueSpan = document.createElement('span');
        valueSpan.className = 'ra-trigger-value';

        var icon = document.createElement('span');
        icon.className = 'ra-calendar-icon';
        icon.setAttribute('aria-hidden', 'true');
        icon.innerHTML = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="5" width="18" height="16" rx="2"/><path d="M3 10h18M8 3v4M16 3v4"/></svg>';

        trigger.appendChild(valueSpan);
        trigger.appendChild(icon);
        linkLabel(input, trigger, valueSpan);

        var panel = document.createElement('div');
        panel.className = 'ra-panel ra-calendar-panel';
        panel.setAttribute('role', 'dialog');
        panel.setAttribute('aria-label', 'Choose a date');
        panel.hidden = true;

        var header = document.createElement('div');
        header.className = 'ra-calendar-header';
        var prevBtn = document.createElement('button');
        prevBtn.type = 'button';
        prevBtn.className = 'ra-calendar-nav';
        prevBtn.setAttribute('aria-label', 'Previous month');
        prevBtn.textContent = '‹';
        var title = document.createElement('span');
        title.className = 'ra-calendar-title';
        var nextBtn = document.createElement('button');
        nextBtn.type = 'button';
        nextBtn.className = 'ra-calendar-nav';
        nextBtn.setAttribute('aria-label', 'Next month');
        nextBtn.textContent = '›';
        header.appendChild(prevBtn);
        header.appendChild(title);
        header.appendChild(nextBtn);

        var weekdays = document.createElement('div');
        weekdays.className = 'ra-calendar-weekdays';
        WEEKDAYS.forEach(function (w) {
            var s = document.createElement('span');
            s.textContent = w;
            weekdays.appendChild(s);
        });

        var grid = document.createElement('div');
        grid.className = 'ra-calendar-grid';

        panel.appendChild(header);
        panel.appendChild(weekdays);
        panel.appendChild(grid);

        input.parentNode.insertBefore(root, input);
        root.appendChild(trigger);
        root.appendChild(panel);
        root.appendChild(input);
        input.classList.add('ra-native-hidden');

        // Same reasoning as the select component: drop `required` from the now
        // display:none input and validate it ourselves via ra:validate.
        var isRequired = input.required;
        input.required = false;

        var min = parseISO(input.getAttribute('min'));
        var selected = parseISO(input.value);
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        var view = selected ? new Date(selected.getFullYear(), selected.getMonth(), 1)
            : new Date(today.getFullYear(), today.getMonth(), 1);

        function refreshValue() {
            valueSpan.textContent = selected ? formatLabel(selected) : 'Select date';
            valueSpan.classList.toggle('is-placeholder', !selected);
            if (isRequired && selected) setInvalid(root, false);
        }

        function render() {
            title.textContent = MONTH_NAMES[view.getMonth()] + ' ' + view.getFullYear();
            grid.innerHTML = '';

            var firstOfMonth = new Date(view.getFullYear(), view.getMonth(), 1);
            var startOffset = (firstOfMonth.getDay() + 6) % 7; // Monday = 0
            var gridStart = new Date(firstOfMonth);
            gridStart.setDate(gridStart.getDate() - startOffset);

            for (var i = 0; i < 42; i++) {
                var cellDate = new Date(gridStart);
                cellDate.setDate(gridStart.getDate() + i);

                var btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'ra-calendar-day';
                btn.textContent = String(cellDate.getDate());

                var outside = cellDate.getMonth() !== view.getMonth();
                var disabled = !!min && cellDate < min;

                if (outside) btn.classList.add('is-outside');
                if (sameDay(cellDate, today)) btn.classList.add('is-today');
                if (sameDay(cellDate, selected)) btn.classList.add('is-selected');
                if (disabled) btn.disabled = true;

                (function (d) {
                    btn.addEventListener('click', function () {
                        selected = d;
                        input.value = toISO(d);
                        dispatchChange(input);
                        refreshValue();
                        close();
                        trigger.focus();
                    });
                })(cellDate);

                grid.appendChild(btn);
            }

            var prevMonthEnd = new Date(view.getFullYear(), view.getMonth(), 0);
            prevBtn.disabled = !!min && prevMonthEnd < min;
        }

        function open() {
            view = selected ? new Date(selected.getFullYear(), selected.getMonth(), 1)
                : new Date(today.getFullYear(), today.getMonth(), 1);
            render();
            panel.hidden = false;
            root.setAttribute('data-state', 'open');
            trigger.setAttribute('aria-expanded', 'true');
            openControl = { root: root, close: close, trigger: trigger };
        }

        function close() {
            panel.hidden = true;
            root.setAttribute('data-state', 'closed');
            trigger.setAttribute('aria-expanded', 'false');
        }

        trigger.addEventListener('click', function () {
            if (root.getAttribute('data-state') === 'open') {
                closeOpenControl();
            } else {
                closeOpenControl();
                open();
            }
        });

        prevBtn.addEventListener('click', function () {
            view = new Date(view.getFullYear(), view.getMonth() - 1, 1);
            render();
        });
        nextBtn.addEventListener('click', function () {
            view = new Date(view.getFullYear(), view.getMonth() + 1, 1);
            render();
        });

        input.addEventListener('ra:validate', function (e) {
            if (isRequired && !input.value) {
                setInvalid(root, true);
                e.preventDefault();
                return;
            }
            setInvalid(root, false);
        });

        refreshValue();
        return root;
    }

    function enhance() {
        document.querySelectorAll('select[data-enhance="select"]').forEach(buildSelect);
        document.querySelectorAll('input[type="date"][data-enhance="date"]').forEach(buildDate);

        // Enhanced controls hide the native element's own constraint validation
        // (browsers skip validation on non-rendered fields), so re-check
        // required fields ourselves before letting a form submit through.
        document.querySelectorAll('form').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                var enhanced = form.querySelectorAll('[data-enhance]');
                var firstInvalid = null;
                enhanced.forEach(function (el) {
                    var ok = el.dispatchEvent(new CustomEvent('ra:validate', { cancelable: true }));
                    if (!ok && !firstInvalid) firstInvalid = el;
                });
                if (firstInvalid) {
                    e.preventDefault();
                    var root = firstInvalid.closest('.ra-control');
                    if (root) root.querySelector('.ra-trigger').focus();
                }
            });
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', enhance);
    } else {
        enhance();
    }
})();

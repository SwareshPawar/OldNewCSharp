window.oldnewSongShell = window.oldnewSongShell || {};

(function (ns) {
  ns.activateFocusTrap = function (selector) {
    ns.__focusTrap = ns.__focusTrap || {};
    var trap = ns.__focusTrap;

    if (typeof trap.dispose === "function") {
      trap.dispose();
    }

    var container = document.querySelector(selector);
    if (!container) {
      return;
    }

    trap.previous = document.activeElement instanceof HTMLElement ? document.activeElement : null;

    var getFocusable = function () {
      return Array.from(
        container.querySelectorAll(
          'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])'
        )
      )
        .filter(function (el) { return el instanceof HTMLElement; })
        .filter(function (el) { return el.offsetParent !== null || el === document.activeElement; });
    };

    var initial = getFocusable()[0] || container;
    if (initial instanceof HTMLElement) {
      initial.focus();
    }

    var onKeyDown = function (ev) {
      if (ev.key !== "Tab") {
        return;
      }

      var focusable = getFocusable();
      if (focusable.length === 0) {
        ev.preventDefault();
        if (container instanceof HTMLElement) {
          container.focus();
        }
        return;
      }

      var first = focusable[0];
      var last = focusable[focusable.length - 1];

      if (ev.shiftKey && document.activeElement === first) {
        ev.preventDefault();
        if (last instanceof HTMLElement) {
          last.focus();
        }
        return;
      }

      if (!ev.shiftKey && document.activeElement === last) {
        ev.preventDefault();
        if (first instanceof HTMLElement) {
          first.focus();
        }
      }
    };

    document.addEventListener("keydown", onKeyDown, true);
    trap.dispose = function () {
      document.removeEventListener("keydown", onKeyDown, true);
    };
  };

  ns.deactivateFocusTrap = function () {
    var trap = ns.__focusTrap;
    if (!trap) {
      return;
    }

    if (typeof trap.dispose === "function") {
      trap.dispose();
      trap.dispose = null;
    }

    var restore = trap.previous;
    trap.previous = null;
    if (restore instanceof HTMLElement) {
      restore.focus();
      return;
    }

    var shell = document.querySelector(".shell");
    if (shell instanceof HTMLElement) {
      shell.focus();
    }
  };

  ns.getPreference = function (key) {
    try {
      return localStorage.getItem(key);
    } catch (_) {
      return null;
    }
  };

  ns.setPreference = function (key, value) {
    try {
      localStorage.setItem(key, value);
    } catch (_) {
      // ignore storage failures
    }
  };

  ns.restoreAndTrackScroll = function (elementId, scrollTop, storageKey) {
    var el = document.getElementById(elementId);
    if (!el) {
      return;
    }

    el.scrollTop = scrollTop;

    if (el.__scrollHandler) {
      el.removeEventListener("scroll", el.__scrollHandler);
    }

    var timeoutId;
    el.__scrollHandler = function () {
      clearTimeout(timeoutId);
      timeoutId = setTimeout(function () {
        try {
          localStorage.setItem(storageKey, String(el.scrollTop));
        } catch (_) {
          // ignore storage failures
        }
      }, 300);
    };

    el.addEventListener("scroll", el.__scrollHandler, { passive: true });
  };

  ns.isInputFocused = function () {
    var el = document.activeElement;
    if (!el) {
      return false;
    }

    var tag = (el.tagName || "").toUpperCase();
    return tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT" || !!el.isContentEditable;
  };

  ns.focusSelector = function (selector) {
    var el = document.querySelector(selector);
    if (el && typeof el.focus === "function") {
      el.focus();
    }
  };

  ns.scrollById = function (elementId, deltaY) {
    var el = document.getElementById(elementId);
    if (el) {
      el.scrollBy(0, deltaY);
    }
  };

  ns.confirmDeleteSetlist = function () {
    return confirm("Delete this setlist?");
  };
})(window.oldnewSongShell);

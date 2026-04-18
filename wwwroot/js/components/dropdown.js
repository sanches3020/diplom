window.AppDropdown = {
  init() {
    document.querySelectorAll('[data-dropdown-toggle]').forEach(toggle => {
      const target = document.querySelector(toggle.dataset.dropdownToggle);
      if (!target) return;

      toggle.addEventListener('click', () => {
        target.classList.toggle('open');
      });
    });
  }
};

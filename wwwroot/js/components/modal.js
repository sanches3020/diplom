window.AppModal = {
  init() {
    document.querySelectorAll('[data-modal-open]').forEach(button => {
      const target = document.querySelector(button.dataset.modalOpen);
      button.addEventListener('click', () => target?.classList.add('open'));
    });

    document.querySelectorAll('[data-modal-close]').forEach(button => {
      const target = button.closest('.modal');
      button.addEventListener('click', () => target?.classList.remove('open'));
    });
  }
};

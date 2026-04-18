window.AppMobileMenu = {
  init() {
    const toggler = document.querySelector('.navbar-toggler');
    const nav = document.querySelector('#mainNavbar');
    if (!toggler || !nav) return;

    toggler.addEventListener('click', () => {
      nav.classList.toggle('show');
    });
  }
};

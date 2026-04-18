window.AppProfileMenu = {
  init() {
    const dropdown = document.querySelector('.profile-menu .dropdown-menu');
    const button = document.querySelector('.profile-menu .profile-button');
    if (!button || !dropdown) return;

    button.addEventListener('click', () => {
      dropdown.classList.toggle('show');
    });

    document.addEventListener('click', (event) => {
      if (!button.contains(event.target) && !dropdown.contains(event.target)) {
        dropdown.classList.remove('show');
      }
    });
  }
};

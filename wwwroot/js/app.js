window.App = {
  init() {
    if (window.AppUtils) {
      AppUtils.updateTitle();
    }

    if (window.AppDropdown) {
      AppDropdown.init();
    }
    if (window.AppModal) {
      AppModal.init();
    }
    if (window.AppProfileMenu) {
      AppProfileMenu.init();
    }
    if (window.AppMobileMenu) {
      AppMobileMenu.init();
    }

    if (window.AppPageHome) {
      AppPageHome.init();
    }
  }
};

window.addEventListener('DOMContentLoaded', function () {
  App.init();
});

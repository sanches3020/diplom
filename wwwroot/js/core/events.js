window.AppEvents = {
  delegate(root, selector, event, handler) {
    root.addEventListener(event, function (e) {
      const target = e.target.closest(selector);
      if (target) {
        handler(e, target);
      }
    });
  }
};

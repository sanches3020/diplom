window.AppUtils = {
  updateTitle(appName = 'Нить сознания') {
    const title = document.title.trim();
    if (!title) {
      document.title = appName;
      return;
    }

    if (title.includes(' - ')) {
      const pageName = title.split(' - ')[0].trim();
      document.title = `${appName} — ${pageName}`;
      return;
    }

    document.title = `${appName} — ${title}`;
  }
};

window.AppApi = {
  get(path) {
    return fetch(path, { credentials: 'same-origin' }).then(res => res.json());
  }
};

window.scrummyJS = {

  scrollToBottom: (elementId) => {
    const el = document.getElementById(elementId);
    el.scrollTop = el.scrollHeight - el.clientHeight
  },

  watchScroll: (elementId, dotnetRef, methodName) => {
    const el = document.getElementById(elementId);
    el.addEventListener('scroll', () => {
      // allow 1px inaccuracy by adding 1
      const bottom = el.scrollHeight - el.clientHeight <= el.scrollTop + 1;
      dotnetRef.invokeMethodAsync(methodName,  bottom);
    });
  },

};

// Тонкий мост между Unity WebGL и MRAID/окружающей страницей.
// Если объекта mraid нет (например, открыт локально или вне SDK-обёртки), все вызовы no-op.
mergeInto(LibraryManager.library, {

  Playable_Init: function () {
    try {
      window.__playable_paused = false;
      window.__playable_muted = false;

      function notify(name) {
        if (typeof unityInstance !== 'undefined' && unityInstance && unityInstance.SendMessage) {
          unityInstance.SendMessage('MraidBridge', name);
        }
      }

      if (typeof mraid !== 'undefined' && mraid) {
        var ready = function () {
          mraid.removeEventListener('ready', ready);
          mraid.addEventListener('viewableChange', function (viewable) {
            notify(viewable ? 'OnResume' : 'OnPause');
          });
          mraid.addEventListener('audioVolumeChange', function (vol) {
            notify(vol === 0 ? 'OnMute' : 'OnUnmute');
          });
        };
        if (mraid.getState && mraid.getState() === 'loading') {
          mraid.addEventListener('ready', ready);
        } else {
          ready();
        }
      } else {
        document.addEventListener('visibilitychange', function () {
          notify(document.hidden ? 'OnPause' : 'OnResume');
        });
      }
    } catch (e) { console.warn('Playable_Init', e); }
  },

  Playable_OpenStore: function (urlPtr) {
    try {
      var url = UTF8ToString(urlPtr);
      if (typeof mraid !== 'undefined' && mraid && mraid.open) {
        mraid.open(url);
      } else {
        window.open(url, '_blank');
      }
    } catch (e) { console.warn('Playable_OpenStore', e); }
  },

  Playable_LogEvent: function (jsonPtr) {
    try {
      var json = UTF8ToString(jsonPtr);
      console.log('[playable]', json);
    } catch (e) { console.warn('Playable_LogEvent', e); }
  }
});

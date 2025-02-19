const jsPlugin = {
  DispatchGameoverEvent: function(score, bestScore, isNewBestScore) {
    window.dispatchReactUnityEvent("Gameover", score, bestScore, isNewBestScore);
  },
  VibrateDevice: function(duration) {
    if (typeof navigator.vibrate === "function") {
      navigator.vibrate(duration);
    }
  },
};

mergeInto(LibraryManager.library, jsPlugin);
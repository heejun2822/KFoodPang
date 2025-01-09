const jsPlugin = {
  DispatchGameoverEvent: function(score, bestScore, isNewBestScore) {
    window.dispatchReactUnityEvent("Gameover", score, bestScore, isNewBestScore);
  },
};

mergeInto(LibraryManager.library, jsPlugin);
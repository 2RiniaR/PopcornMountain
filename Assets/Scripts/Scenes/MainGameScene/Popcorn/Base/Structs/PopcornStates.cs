namespace PopcornMountain.MainGameScene.Popcorn {
    public enum PopcornCookingStates {
        Bean,    // 豆の状態(初期状態)
        Popped,  // 弾けた状態
        Burned,  // 焦げた状態
    }

    public enum PopcornCatchingStates {
        Uncatchable, // 獲得できない状態(初期状態)
        Catchable,   // 獲得可能な状態
    }
}

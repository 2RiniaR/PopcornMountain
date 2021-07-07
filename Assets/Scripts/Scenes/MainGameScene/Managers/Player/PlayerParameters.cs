namespace PopcornMountain.MainGameScene {

    public enum PlayerID {
        /// <summary>
        ///   自分
        /// </summary>
        Self,

        /// <summary>
        ///   相手
        /// </summary>
        Other
    }


    public struct PlayerParameters {

        /// <summary>
        ///   プレイヤーのID
        /// </summary>
        public readonly PlayerID playerID;

        /// <summary>
        ///   プレイヤーのニックネーム
        /// </summary>
        public string nickname;

        /// <summary>
        ///   最大の満腹度
        /// </summary>
        public readonly float maxHungerPoint;

        public PlayerParameters(
            PlayerID id,
            string nickname,
            float maxHungerPoint
        ) {
            this.playerID = id;
            this.nickname = nickname;
            this.maxHungerPoint = maxHungerPoint;
        }

    }
}

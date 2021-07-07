using System;


public enum PopcornType {
    Normal,
    Hard,
    Bomber,
    Golden,
    Danger,
    Blaze,
    Soggy,
    Troll
}


[Serializable]
public struct PopcornParameters {

    /// <summary>
    ///   種類
    /// </summary>
    public PopcornType type;

    /// <summary>
    ///   破裂するときの熱量
    /// </summary>
    public float popHeat;

    /// <summary>
    ///   焦げるときの熱量
    /// </summary>
    public float blackHeat;

    /// <summary>
    ///   破裂時の爆発力
    /// </summary>
    public float explosionPower;

    /// <summary>
    ///   取得時に増加する満腹度
    /// </summary>
    public float point;

    /// <summary>
    ///   爆発の威力減少量
    /// </summary>
    public float diffence;

    /// <summary>
    ///   水蒸気の発生量
    /// </summary>
    public float steam;

    /// <summary>
    ///   スケール
    /// </summary>
    public float scale;

    /// <summary>
    ///   破裂前の質量
    /// </summary>
    public float unpoppedMass;

    /// <summary>
    ///   破裂後の質量
    /// </summary>
    public float poppedMass;

}

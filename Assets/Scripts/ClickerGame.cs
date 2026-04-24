using UnityEngine;
using TMPro;
using System.Collections; // コルーチン(IEnumerator)を使うために必須

public class ClickerGame : MonoBehaviour
{
    // --- 内部データ（private：インスペクターには出さない） ---
    private int score = 0;              // 現在の合計スコア
    private int power = 1;              // 1クリックあたりの増加量
    private int currentItemCost;        // 次に購入する際のアイテム価格
    private Coroutine fadeCoroutine;    // 実行中のフェード処理を管理する変数

    // --- Unity上で紐付けるUI要素（public：インスペクターで設定） ---
    public TextMeshProUGUI scoreText;       // スコア表示用
    public TextMeshProUGUI messageText;     // 警告・通知メッセージ用
    public TextMeshProUGUI buyButtonText;   // 購入ボタンの文字書き換え用
    public TextMeshProUGUI powerText;       // 現在のクリック力表示用
    public ItemData upgradeItem;            // ScriptableObject（アイテムの設計図）

    // ゲーム開始時に1回だけ呼ばれる
    void Start()
    {
        // 最初の価格を設計図（ItemData）からコピーして持ってくる
        currentItemCost = upgradeItem.baseCost;
        UpdateUI(); // 最初の画面表示を更新
    }

    // メインのボタンが押された時に実行（ButtonのOnClickに設定）
    public void OnButtonClick()
    {
        score += power; // スコアを増やす
        UpdateUI();     // 画面を書き換える
    }

    // 購入ボタンが押された時に実行（BuyButtonのOnClickに設定）
    public void BuyUpgrade()
    {
        // お金が足りているかチェック
        if (score >= currentItemCost)
        {
            score -= currentItemCost;           // お金を支払う
            power += upgradeItem.scorePerClick; // クリック力をアップ

            // 次回の価格を計算（1.5倍にして、小数点以下を切り上げ）
            currentItemCost = Mathf.CeilToInt(currentItemCost * 1.5f);

            UpdateUI(); // 画面を書き換える
            ShowMessage(upgradeItem.itemName + "を購入！");
        }
        else
        {
            // 足りない場合は警告。現在の価格を教えてあげる親切設計
            ShowMessage("スコアが足りません（必要: " + currentItemCost + "）");
        }
    }

    // 画面上のあらゆる文字を最新の状態に更新する関数
    void UpdateUI()
    {
        scoreText.text = "Score: " + score.ToString();

        if (powerText != null)
            powerText.text = "1クリック: " + power.ToString() + "点";

        if (buyButtonText != null)
            buyButtonText.text = upgradeItem.itemName + "を買う\n(" + currentItemCost + "点)";
    }

    // メッセージを表示する入り口
    void ShowMessage(string text)
    {
        // もし既に消えている途中のメッセージがあれば中断させる（チカチカ防止）
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        // 新しくフェードアウト処理を開始
        fadeCoroutine = StartCoroutine(FadeOutMessage(text));
    }

    // 数秒かけて文字を消す「時間の流れ」を持つ処理
    IEnumerator FadeOutMessage(string text)
    {
        messageText.text = text;
        messageText.alpha = 1f; // 完全に表示（不透明）

        yield return new WaitForSeconds(1.0f); // 1秒間、表示したまま待機

        float duration = 1.0f; // 1秒かけて消す設定
        float currentTime = 0f;

        // duration（1秒）経つまでループを回す
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime; // 前のフレームからの経過時間を加算
            // Mathf.Lerpを使って、徐々に1から0へ透明度を変化させる
            messageText.alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            yield return null; // 1フレーム待ってからループの最初に戻る
        }

        messageText.text = ""; // 最後は空文字にしておく
    }
}
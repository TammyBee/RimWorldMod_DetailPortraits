# Detail Portraits
- 画面上部の入植者バーのポートレイトに好きな画像を表示できるようになるModです。  
- 前提MODとして「[Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)」が必要です。
- 入植者バーのポートレイトのサイズを変更できるMod「[Custom Portraits](https://steamcommunity.com/sharedfiles/filedetails/?id=1569605867)」の導入を推奨しています。  
- 詳しい説明は[こちら](https://github.com/TammyBee/RimWorldMod_DetailPortraits/wiki)。
- 紹介動画は[こちら](https://www.nicovideo.jp/watch/sm35984080)。
- サンプルとして設定済みのプリセット「DetailPortraits.xml」を用意しました。  
使いたい場合は、以下のフォルダ内に入れてください(環境によって異なる場合があります)。  
「C:\Users\<ユーザー名>\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Config」  
また、すでにプリセットを保存している場合、そのまま上書きするとそのプリセットが消えてしまうため、  
バックアップをするか、「DetailPortraits.xml」をうまい具合に編集しましょう。  


## 更新履歴
[2020/0X/XX]機能追加(1.0.5)  
・描画条件「精神崩壊リスク」を追加  
・描画条件「空腹」を追加  
・描画条件「退屈」を追加  
・描画条件「治療が必要」を追加  
・描画条件「救助が必要」を追加  
・描画条件「能力」を追加  
・描画条件「現在時刻(時)」を追加  
・描画条件「平均発生間隔」を追加  
・一部の描画条件をグループ化しました。  
・レイヤーの画像ファイルのパスの先頭に「!」を付けた場合、基準パスを先頭に追加しないようにしました。  
・MOD設定を追加  
・描画条件「精神状態」の演算子「is empty」のバグを修正  
・描画条件「乱数」が非推奨になりました(描画条件「平均発生間隔」の利用を推奨)  

[2020/03/28]機能追加(1.0.4)  
・描画条件「Jobのターゲット」を追加  
・ポートレイト設定「画像ファイルの基準パス」を追加  
・画像の拡大率を自動で調整するようにしました(MOD設定から変更可能)。  
・MOD設定を追加  

[2020/03/01]バグ修正(1.0.3)  
・起動時に出るバグを修正  
・Harmonyを前提MODにしました。   

[2020/02/27]1.0と1.1に両対応(1.0.2)  
・1.0と1.1に両対応  
・サンプルとして設定済みのプリセットと画像を追加しました。  
・プリセットのロードに関するバグを修正  
・死んだ入植者のポートレイトが保存されていないバグを修正  
・座標において小数第一位までしか保存されていないバグを修正   

[2019/11/22]機能追加+バグ修正(1.0.1)  
・縦方向と横方向の拡大率を個別に変更できるようにした。  
・レイヤーを有効化/無効化できるようにした。  
・アイコンを隠す設定をした時、死亡時のマークも消すように変更。  
・レイヤーのコピーに関するバグを修正。  
・特定の状況下でのバグを修正。   

[2019/11/16]バグ修正(1.0.0b)  
・入植者が追加されるときに発生するバグを修正。  
・左辺がJobの描画条件を持つレイヤーを持つ入植者がキャラバンに加わると発生するバグを修正。  
・レイヤーのコピーに関するバグを修正。  

[2019/10/10]公開(1.0.0)  


## その他  
質問、不具合などがあれば、Twitter(@tammybee_tmb)か以下のページのコメント欄、RimMod相談所の#民火modにて報告をお願いします。

- [民火MOD - RimWorld私的wiki](http://seesaawiki.jp/rimworld/d/%cc%b1%b2%d0%20MOD)  
- [RimMod相談所 - Discord](https://discordapp.com/invite/WfVjNxs)  

このMODはHarmony Patch Libraryを使用しています。  

- [Harmonyスレッド | Ludeonフォーラム](https://ludeon.com/forums/index.php?topic=29517.0)  

"Twemoji" by Twitter, Inc and other contributors is licensed under CC-BY 4.0

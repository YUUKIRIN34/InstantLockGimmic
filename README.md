# 簡易ワールド固定ギミック

## 導入

1. Modular Avatarを導入する
2. YUUKIRIN_InstantLockGimmic_vXXX.unitypackageをimportする

## 使い方

1. ワールド固定するアイテムをシーンに展開
2. YUUKIRIN→InstantLockGimmicでウィンドウを表示
3. ロックオフ時に追従するボーンを指定（右手など）
4. ロックするオブジェクトにワールド固定するアイテムを指定
5. 適用をクリック

## 補足

- ロックするオブジェクトは指定しなくてもよい
    - 指定しない場合はContainer内に直接投入する
- Expression, Animationで次の変数を利用します
    - lock
    - showLock

# License

This project is licensed under the MIT License, see the LICENSE file for details
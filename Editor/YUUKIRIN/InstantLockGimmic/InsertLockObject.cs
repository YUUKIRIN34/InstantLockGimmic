using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Constraint.Components;

// using VRC.Dynamics;

public partial class Yuukirin_InstantLockGimmic : EditorWindow {

    [MenuItem("YUUKIRIN/簡易ロックギミック")]
    private static void ShowWindow()
    {
        var window = GetWindow<Yuukirin_InstantLockGimmic>("UIElements");
        window.titleContent = new GUIContent("ゆうきりん/簡易ロックギミック");
        window.Show();
    }
    private ObjectField YUUKIRINLockGimmicTargetField;
    private ObjectField YUUKIRINLockGimmicChildField;
    private Toggle YUUKIRINLockGimmicPlaceField;
    private Button button;
    private HelpBox YUUKIRINLockGimmicTargetErrorElement;
    private HelpBox YUUKIRINLockGimmicResultElement;
    
    private void CreateGUI() {
        VisualElement root = rootVisualElement;
        Label YUUKIRINLockGimmicTargetLabelElement = new(){
            text="ロックオフ時に追従するボーン"
        };
        YUUKIRINLockGimmicTargetField = new(){
            objectType = typeof(GameObject),
        };
        YUUKIRINLockGimmicTargetErrorElement = new(){
            messageType = HelpBoxMessageType.Error,
            text = "ロックオフ時に追従するオブジェクトが指定されていません。",
            visible = false
        };
        Label YUUKIRINLockGimmicChildLabelElement = new(){
            text="ロックするオブジェクト"
        };
        YUUKIRINLockGimmicChildField = new(){
            objectType = typeof(GameObject)
        };
        Label YUUKIRINLockGimmicPlaceLabelElement = new(){
            text="ロック時のみ表示する"
        };
        YUUKIRINLockGimmicPlaceField = new(){};
        button = new(){
            text="適用"
        };
        YUUKIRINLockGimmicTargetField.RegisterValueChangedCallback(_ => ValidateRequired(YUUKIRINLockGimmicTargetField, YUUKIRINLockGimmicTargetErrorElement));
        YUUKIRINLockGimmicResultElement = new(){
            messageType = HelpBoxMessageType.Info,
            text = "成功",
            visible = false
        };

        root.Add(YUUKIRINLockGimmicTargetLabelElement);
        root.Add(YUUKIRINLockGimmicTargetField);
        root.Add(YUUKIRINLockGimmicTargetErrorElement);
        root.Add(YUUKIRINLockGimmicPlaceLabelElement);
        root.Add(YUUKIRINLockGimmicPlaceField);
        root.Add(YUUKIRINLockGimmicChildLabelElement);
        root.Add(YUUKIRINLockGimmicChildField);
        root.Add(button);
        root.Add(YUUKIRINLockGimmicResultElement);
        button.clicked += () => {
            CreateLockGimmic();
        };
    }

    private bool ValidateRequired<T>(BaseField<T> field, HelpBox helpBox) {
        T value = (T)field.value;
        helpBox.visible = value == null;
        SetResultVisible(false);
        return SetButtonEnable();
    }

    private bool YUUKIRINLockGimmicTargetFieldFilled () {
        Object value = (Object)YUUKIRINLockGimmicTargetField.value;
        if (!value) {
            return false;
        } else {
            return true;
        }
    }

    private bool SetButtonEnable () {
        button.SetEnabled(YUUKIRINLockGimmicTargetFieldFilled());
        return YUUKIRINLockGimmicTargetFieldFilled();
    }

    private void SetResultVisible (bool val) {
        YUUKIRINLockGimmicResultElement.visible = val;
    }

    private bool ValidateAll () {
        ValidateRequired<Object>(YUUKIRINLockGimmicTargetField, YUUKIRINLockGimmicTargetErrorElement);
        SetResultVisible(false);
        return SetButtonEnable();
    }

    private void CreateLockGimmic() {
        if (!ValidateAll()) {
            return;
        }
        GameObject target = (GameObject)YUUKIRINLockGimmicTargetField.value;
        // ルートオブジェクト
        GameObject rootObject = target.transform.root.gameObject;
        // prefab
        string path = YUUKIRINLockGimmicPlaceField.value ? "Assets/Editor/YUUKIRIN/InstantLockGimmic/Prefab/LockGimmicObject_place.prefab" : "Assets/Editor/YUUKIRIN/InstantLockGimmic/Prefab/LockGimmicObject.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject LockGimmicObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (prefab == null) {Debug.Log("prefabがない");}
        if (LockGimmicObject == null) {Debug.Log("展開できない");}
        LockGimmicObject.transform.parent = rootObject.transform;
        LockGimmicObject.transform.localPosition = Vector3.zero;
        /*
        * Add GameObject
        */
        // ObjectPoint
        GameObject ObjectPoint = new("ObjectPoint");
        ObjectPoint.transform.parent = target.transform;
        ObjectPoint.transform.localPosition = Vector3.zero;

        /*
        * Add ParentConstraint
        */

        // Lock 
        GameObject lockPoint = LockGimmicObject.transform.Find("LockGimmic/LockPoint").gameObject;
        VRCParentConstraint lockPointParentConstraint = lockPoint.GetComponent<VRCParentConstraint>();
        VRCConstraintSource source = lockPointParentConstraint.Sources[0];
        source.SourceTransform = ObjectPoint.transform;
        lockPointParentConstraint.Sources[0] = source;

        // ロック対象
        if (YUUKIRINLockGimmicChildField.value) {
            // Container
            GameObject Container = LockGimmicObject.transform.Find("LockGimmic/Container").gameObject;
            GameObject child = (GameObject)YUUKIRINLockGimmicChildField.value;
            // ロック対象を指定したなら親子設定
            child.gameObject.transform.parent = Container.transform;
        }

        SetResultVisible(true);
    }
}
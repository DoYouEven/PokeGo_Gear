using UnityEngine;
using System.Collections.Generic;

using FirebaseCSharp;
using System.Collections;

public class SampleScript : MonoBehaviour
{

    static int debug_idx = 0;

    [SerializeField]
    TextMesh textMesh;

    Firebase firebase;
    Firebase frb_child;

    // Use this for initialization
    void Start()
    {
        textMesh.text = "";

        firebase = Firebase.CreateNew("pokego-gear.firebaseio.com");
        frb_child = firebase.Child("key14").Child("key21");


        DoDebug("Firebase endpoint: " + firebase.Endpoint);
        DoDebug("Firebase key: " + firebase.Key);
        DoDebug("Firebase fullKey: " + firebase.FullKey);
        DoDebug("Firebase child key: " + frb_child.Key);
        DoDebug("Firebase child fullKey: " + frb_child.FullKey);

        firebase.OnFetchSuccess += GetOKHandler;
        firebase.OnFetchFailed += GetFailHandler;
        firebase.OnUpdateSuccess += SetOKHandler;
        firebase.OnUpdateFailed += SetFailHandler;
        firebase.OnPushSuccess += PushOKHandler;
        firebase.OnPushFailed += PushFailHandler;

        frb_child.OnDeleteSuccess += DelOKHandler;
        frb_child.OnDeleteFailed += DelFailHandler;


        Dictionary<string, object> dict1 = new Dictionary<string, object>();
        dict1.Add("key11", false);
        dict1.Add("key12", 0.123);
        dict1.Add("key13", 1000);

        Dictionary<string, object> dict2 = new Dictionary<string, object>();
        dict2.Add("key21", "this will be deleted later");
        dict2.Add("key22", "dkrprasetya.github.com/simple-firebase-csharp");

        dict1.Add("key14", dict2);

        // NOTE:
        // Wrap your firebase methods with Unity's coroutine manually or use Firebase.ToCoroutine() to make it behaves Asynchronus-ish
        // if not, your script will wait until the method's completed    
        
        //// Test #1: Set & Get
        StartCoroutine(Firebase.ToCoroutine(firebase.SetValue, dict1));
        StartCoroutine(Firebase.ToCoroutine(firebase.GetValue));

        //// Test #2: Set child & get with manual coroutine
        StartCoroutine(TestCoroutine());

        //// Test #3: Push value (with json string) & get with FirebaseParam parameter
        firebase.Child("Messages").Push("{ \"name\": \"simple-firebase-csharp\", \"message\": \"awesome!\"}", true); // note that the push action handler is not called, as the action is assigned only to current key, not to its child

        //// Test #4: This one is deliberately made that it should be error because of using Shallow keyword and Filtering parameter at the same time
        DoDebug("This one error request is deliberately made that it should be error\nbecause of using Shallow parameter and Filtering parameter at the same time.");
        StartCoroutine(Firebase.ToCoroutine(firebase.GetValue, FirebaseParam.Empty.OrderByKey().Shallow()));

        //// Test #5: Delete value & get with string parameter
        StartCoroutine(Firebase.ToCoroutine(firebase.Child("key12").Delete));
        StartCoroutine(Firebase.ToCoroutine(frb_child.Delete));
        StartCoroutine(Firebase.ToCoroutine(firebase.GetValue, "print=pretty"));

        //// Delete pushed messages so that users' message not piling-up on my DB...
        firebase.Child("Messages").Delete();
    }

    void OnDisable()
    {
        firebase.OnFetchSuccess -= GetOKHandler;
        firebase.OnFetchFailed -= GetFailHandler;
        firebase.OnUpdateSuccess -= SetOKHandler;
        firebase.OnUpdateFailed -= SetFailHandler;
        firebase.OnPushSuccess -= PushOKHandler;
        firebase.OnPushFailed -= PushFailHandler;

        frb_child.OnDeleteSuccess -= DelOKHandler;
        frb_child.OnDeleteFailed -= DelFailHandler;
    }

    void GetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Get from key: <" + sender.FullKey + ">");
        DoDebug("[OK] Raw Json: " + snapshot.RawJson);

        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (keys != null)
            foreach (string key in keys)
            {
                DoDebug(key + " = " + dict[key].ToString());
            }
    }

    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void DelOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Del from key: <" + sender.FullKey + ">");
    }

    void DelFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Del from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void PushOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Push from key: <" + sender.FullKey + ">");
    }

    void PushFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Push from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void DoDebug(string str)
    {
        Debug.Log(str);
        if (textMesh != null)
        {
            textMesh.text += (++debug_idx + ". " + str) + "\n";
        }
    }

    IEnumerator TestCoroutine()
    {
        firebase.Child("Test").SetValue(123);

        yield return null;

        firebase.GetValue();

        yield return null;
    }

}

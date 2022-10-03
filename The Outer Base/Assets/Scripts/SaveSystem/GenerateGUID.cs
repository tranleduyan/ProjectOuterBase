using UnityEngine;

[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            //ensure the object has a guaranteed unique id
            if(_gUID == "")
            {
                //assign GUID
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}

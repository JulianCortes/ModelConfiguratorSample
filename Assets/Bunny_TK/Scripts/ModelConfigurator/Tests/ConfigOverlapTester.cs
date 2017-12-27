using Bunny_TK.ModelConfigurator;
using UnityEngine;

[ExecuteInEditMode]
public class ConfigOverlapTester : MonoBehaviour
{
    public ConfigurationID a;
    public ConfigurationID b;
    public ConfigurationID result;
    public bool overlap = false;
    public bool checkEquality = false;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (overlap)
        {
            result.Overlap(a, b);
            overlap = false;
        }

        if (checkEquality)
        {
            print(a.Similar(b.configValues));
            checkEquality = false;
        }
    }
}
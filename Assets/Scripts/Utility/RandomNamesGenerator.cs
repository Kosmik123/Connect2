#if UNITY_EDITOR
using System.Text;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu]
public class RandomNamesGenerator : ScriptableObject
{
    [SerializeField]
    private string vowels = "aeiouy";
    [SerializeField]
    private string consonants = "bcdfghjklmnpqrstvwxz";

    [SerializeField]
    private int minLength;
    [SerializeField]
    private int maxLength;

    private int length;
    private bool is2AgoVowel, isLastVowel;

    public bool IsConsonant(char c)
    {
        return consonants.IndexOf(c) >= 0;
    }

    public bool IsVowel(char c)
    {
        return vowels.IndexOf(c) >= 0;
    }

    public string GetRandomName()
    {
        length = Random.Range(minLength, maxLength + 1);

        var builder = new StringBuilder();

        builder.Append(GetRandomLetter(out is2AgoVowel));
        builder.Append(GetRandomLetter(out isLastVowel));
        
        for (int index = 0; index < length; index++)
        {
            if(is2AgoVowel && isLastVowel)
            {
                is2AgoVowel = isLastVowel;
                builder.Append(GetRandomLetterFrom(consonants, out isLastVowel));
            }
            else if(is2AgoVowel != isLastVowel)
            {
                is2AgoVowel = isLastVowel;
                builder.Append(GetRandomLetter(out isLastVowel));
            }
            else
            {
                is2AgoVowel = isLastVowel;
                builder.Append(GetRandomLetterFrom(vowels, out isLastVowel));
            }
        }
        return builder.ToString();
    }

    private string GetRandomLetterFrom(string set, out bool isVowel)
    {
        isVowel = (set == vowels);
        int index = Random.Range(0, set.Length);
        return set[index].ToString();
    }

    private string GetRandomLetter(out bool isVowel)
    {
        int opt = Random.Range(0, 2);
        if(opt == 0)
            return GetRandomLetterFrom(vowels, out isVowel);
        return GetRandomLetterFrom(consonants, out isVowel);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RandomNamesGenerator))]
    public class RandomNamesGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Test Generate Name"))
            {
                var generator = target as RandomNamesGenerator;
                Debug.Log(generator.GetRandomName());
            }
        }
    }
#endif
}

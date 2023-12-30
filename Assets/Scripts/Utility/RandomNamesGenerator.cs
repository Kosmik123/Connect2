#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class RandomNamesGenerator : MonoBehaviour
{
    public string vowels = "aeiouy";
    public string consonants = "bcdfghjklmnpqrstvwxz";
    
    public int minLength;
    public int maxLength;

    private int length;
    private int index;
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
        index = 0;
        string name = "";

        name += GetRandomLetter(out is2AgoVowel);
        name += GetRandomLetter(out isLastVowel);
        while(index < length)
        {
            if(is2AgoVowel && isLastVowel)
            {
                is2AgoVowel = isLastVowel;
                name += GetRandomLetterFrom(consonants, out isLastVowel);
            }
            else if(is2AgoVowel != isLastVowel)
            {
                is2AgoVowel = isLastVowel;
                name += GetRandomLetter(out isLastVowel);
            }
            else
            {
                is2AgoVowel = isLastVowel;
                name += GetRandomLetterFrom(vowels, out isLastVowel);
            }
            index++;
        }
        return name;
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
                RandomNamesGenerator gen = target as RandomNamesGenerator;
                Debug.Log(gen.GetRandomName());
            }
        }
    }
#endif
}

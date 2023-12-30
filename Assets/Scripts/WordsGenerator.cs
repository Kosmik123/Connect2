using NaughtyAttributes;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface IChance
{
    int Chance { get; set; }
}

[System.Serializable]
public class ObjectChance<T> : IChance
{
    [field: SerializeField]
    public T Object { get; set; }
    
    [field: SerializeField]
    public int Chance { get; set; }

    public ObjectChance(T @object, int chance) 
    {
        Object = @object;
        Chance = chance;
    }

    public ObjectChance(T @object) : this(@object, 1)
    { }

    public ObjectChance() : this(default, 1)
    { }

    public override string ToString()
    {
        return $"{Object}: {Chance}";
    }
}

public class CharacterChance : ObjectChance<char>
{
    public CharacterChance() : base()
    { }

    public char Character => Object;
}

public class CountChance : ObjectChance<int>
{
    public CountChance() : base()
    { }

    public int Count => Object;
}


[CreateAssetMenu]
public class WordsGenerator : ScriptableObject
{
    public enum Separator
    {
        Space,
        NewLine,
        Coma,
        Semicolon,
    }

    [SerializeField]
    private TextAsset exampleWordsFile;
    [SerializeField]
    private Separator separator;

    private string[] exampleWords;
    public IReadOnlyList<string> ExampleWords
    {
        get
        {
            if (exampleWords == null || exampleWords.Length < 1)
                CreateExampleWords();
            return exampleWords;
        }
    }

    private CharacterChance[] firstCharacterChances;
    private CharacterChance[] anyCharacterChances;

    private Dictionary<char, List<CharacterChance>> characterChancesByPreviousCharacter;
    public Dictionary<char, List<CharacterChance>> CharacterChancesByPreviousCharacter
    {
        get
        {
            if (characterChancesByPreviousCharacter == null)
                CreateChancesDict();
            return characterChancesByPreviousCharacter;
        }
    }

    private Dictionary<char, Dictionary<char, List<CharacterChance>>> characterChancesByTwoPreviousCharacters;
    public Dictionary<char, Dictionary<char, List<CharacterChance>>> CharacterChancesByTwoPreviousCharacters
    {
        get
        {
            if (characterChancesByTwoPreviousCharacters == null)
                CreateTwoCharacterChancesDict();
            return characterChancesByTwoPreviousCharacters;
        }
    }

    private List<CountChance> _countChances;
    public List<CountChance> CountChances
    {
        get
        {
            if (_countChances == null || _countChances.Count < 1)
                CreateCountChances(ref _countChances);
            return _countChances;
        }
    }

    private void CreateChancesDict()
    {
        characterChancesByPreviousCharacter = new Dictionary<char, List<CharacterChance>>();
        var words = ExampleWords;
        foreach (var word in words)
            ProcessWord(characterChancesByPreviousCharacter, word);
    }

    private void CreateTwoCharacterChancesDict()
    {
        characterChancesByTwoPreviousCharacters = new Dictionary<char, Dictionary<char, List<CharacterChance>>>();
        var words = ExampleWords;
        foreach (var word in words)
            ProcessWord(characterChancesByTwoPreviousCharacters, word);

        //foreach (var dictsByLastChar in characterChancesByTwoPreviousCharacters)
        //{
        //    char lastChar = dictsByLastChar.Key;
        //    foreach (var chancesByCharBefore in dictsByLastChar.Value)
        //    {
        //        char charBeforeLast = chancesByCharBefore.Key;
        //        foreach (var chance in chancesByCharBefore.Value)
        //        {
        //            Debug.Log($"Po znakach '{charBeforeLast}{lastChar}' mo¿e byæ '{chance.Character}'  z szans¹ {chance.Chance}");
        //        }
        //    }
        //}
    }

    private void CreateCountChances(ref List<CountChance> chances)
    {
        chances = new List<CountChance>();
        var words = ExampleWords;
        foreach (var word in words)
            AddChance(chances, word.Length);
    }

    private void Reset()
    {
        characterChancesByTwoPreviousCharacters = null;
        exampleWords = null;
        characterChancesByPreviousCharacter = null;
        firstCharacterChances = null;
        anyCharacterChances = null;
        _countChances = null;
    }

    private void CreateExampleWords()
    {
        var wordsList = new List<string>(exampleWordsFile.text.Split(GetSeparator(separator)));
        for (int i = 0; i < wordsList.Count; i++)
            wordsList[i] = wordsList[i].Trim();

        for (int i = wordsList.Count - 1; i >= 0; i--)
            if (string.IsNullOrWhiteSpace(wordsList[i]))
                wordsList.RemoveAt(i);

        exampleWords = wordsList.ToArray();
    }

    private static void ProcessWord(Dictionary<char, List<CharacterChance>> chancesDict, string word)
    {
        int count = word.Length;
        for (int i = 0; i < count - 1; i++)
        {
            char character = word[i];
            char nextCharacter = word[i + 1];

            AddCharacterChance(chancesDict, character, nextCharacter);
        }
    }

    private static void AddCharacterChance(Dictionary<char, List<CharacterChance>> chancesDict, char character, char nextCharacter)
    {
        character = char.ToLower(character);
        if (chancesDict.ContainsKey(character) == false)
            chancesDict.Add(character, new List<CharacterChance>());

        var chances = chancesDict[character];
        AddCharacterChance(chances, nextCharacter);
    }

    private static void AddCharacterChance(List<CharacterChance> chances, char character)
    {
        character = char.ToLower(character);
        AddChance(chances, character);
    }

    private static void AddChance<TChance, TObject>(List<TChance> chances, TObject @object) where TChance : ObjectChance<TObject>, new()
    {
        int chanceIndex = chances.FindIndex(charChance => charChance.Object.Equals(@object));
        if (chanceIndex < 0)
        {
            var ch = new TChance()
            {
                Object = @object,
                Chance = 1,
            };
            chances.Add(ch);
        }
        else
        {
            var ch = chances[chanceIndex];
            ch.Chance += 1;
            chances[chanceIndex] = ch;
        }
    }

    private static void ProcessWord(Dictionary<char, Dictionary<char, List<CharacterChance>>> chancesDict, string word)
    {
        int count = word.Length;
        for (int i = 0; i < count - 2; i++)
        {
            char characterBefore = word[i];
            char character = word[i + 1];
            char nextCharacter = word[i + 2];
            if (chancesDict.ContainsKey(character) == false)
                chancesDict.Add(character, new Dictionary<char, List<CharacterChance>>());

            var chancesByCharacterBefore = chancesDict[character];
            AddCharacterChance(chancesByCharacterBefore, characterBefore, nextCharacter);
        }
    }

    public char GetSeparator(Separator separator)
    {
        return separator switch
        {
            Separator.NewLine => '\n',
            Separator.Coma => ',',
            Separator.Semicolon => ';',
            _ => ' ',
        };
    }

    public char GetNextChar(char lastCharacter)
    {
        if (CharacterChancesByPreviousCharacter.TryGetValue(lastCharacter, out var chances) == false)
            return default;

        return GetCharacter(chances);
    }

    public char GetNextChar(char beforeLastCharacter, char lastCharacter)
    {
        if (beforeLastCharacter == default)
            return GetNextChar(lastCharacter);

        if (CharacterChancesByTwoPreviousCharacters.TryGetValue(lastCharacter, out var nextCharByCharBeforeLast) == false)
            return GetNextChar(lastCharacter);

        if (nextCharByCharBeforeLast.TryGetValue(beforeLastCharacter, out var chances) == false)
            return GetNextChar(lastCharacter);

        return GetCharacter(chances);
    }

    public char GetFirstCharacter()
    {
        if (firstCharacterChances == null || firstCharacterChances.Length == 0)
            CreateFirstCharacterChances();
        return GetCharacter(firstCharacterChances);
    }

    public char GetRandomCharacter()
    {
        if (anyCharacterChances == null || anyCharacterChances.Length == 0)
            CreateAnyCharacterChances();
        return GetCharacter(anyCharacterChances);
    }

    private void CreateAnyCharacterChances()
    {
        var words = ExampleWords;
        var anyCharacterChancesList = new List<CharacterChance>();
        foreach (var word in words)
            foreach (char character in word)
                AddCharacterChance(anyCharacterChancesList, character);

        anyCharacterChances = anyCharacterChancesList.ToArray();
    }

    private void CreateFirstCharacterChances()
    {
        var words = ExampleWords;
        var firstCharacterChancesList = new List<CharacterChance>();
        foreach (var word in words)
        {
            char character = word[0];
            AddCharacterChance(firstCharacterChancesList, character);
        }

        firstCharacterChances = firstCharacterChancesList.ToArray();
    }

    public char GetNextChar(string text)
    {
        if (text == null || text.Length == 0)
            return GetFirstCharacter();

        int count = text.Length;
        if (count < 2)
            return GetNextChar(text[0]);

        return GetNextChar(text[count - 2], text[count - 1]);
    }

    public static char GetCharacter(IReadOnlyList<CharacterChance> chances) => GetChanceObject<CharacterChance, char>(chances);
    private int GetRandomCount() => GetChanceObject<CountChance, int>(CountChances);

    public static TObject GetChanceObject<TChance, TObject>(IReadOnlyList<TChance> chances) where TChance : ObjectChance<TObject>
    {
        int maxNumber = 0;
        for (int i = 0; i < chances.Count; i++)
            maxNumber += chances[i].Chance;

        int chosenIndex = Random.Range(0, maxNumber);
        int currentIndex = 0;
        for (int i = 0; i < chances.Count; i++)
        {
            currentIndex += chances[i].Chance;
            if (chosenIndex < currentIndex)
                return chances[i].Object;
        }

        return chances[0].Object;
    }

    [Button]
    public void GenerateWords()
    {
        for (int i = 0; i < 100; i++)
            GenerateWord();
    }

    [Button]
    public void GenerateWord()
    {
        int count = GetRandomCount();

        var builder = new StringBuilder();
        char character = GetFirstCharacter();
        char characterBefore = default;

        builder.Append(character);
        for (int i = 0; i < count; i++)
        {
            char newCharacter = GetNextChar(characterBefore, character);
            if (newCharacter == default)
                break;

            builder.Append(newCharacter);
            characterBefore = character;
            character = newCharacter;
        }

        string word = builder.ToString();
        word = $"{char.ToUpper(word[0])}{word.Substring(1)}";
        Debug.Log(word);
    }

}

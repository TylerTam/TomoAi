using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TomoCharEmotions : MonoBehaviour
{
    [SerializeField] public EmotionAnalysis.Emotion MainEmotion;
#if UNITY_EDITOR
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<EmotionAnalysis.Emotion, float> debugEmotions = new RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<EmotionAnalysis.Emotion, float>();
#endif
    [SerializeField] private bool playParticleOnReaction;
    [SerializeField] private bool alwaysPlayEmotionParticle;
    private Dictionary<EmotionAnalysis.Emotion, float> currentEmotions = new Dictionary<EmotionAnalysis.Emotion, float>();

    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<EmotionAnalysis.Emotion, GameObject> emotionParticles;
    public System.Action MainEmotionUpdated;
    private bool skipReaction;

    private void Awake()
    {
        MainEmotionUpdated += PlayCurrentEmotionParticle;
    }
    public void AddEmotion(EmotionAnalysis emotionAnalysis)
    {
        foreach (KeyValuePair<EmotionAnalysis.Emotion, float> key in emotionAnalysis.score)
        {
            AddToEmotion(key.Key, key.Value);
        }
        EmotionAnalysis.Emotion newEmotion = MeasureEmotions(currentEmotions);
        if (newEmotion != MainEmotion || alwaysPlayEmotionParticle)
        {
            MainEmotion = newEmotion;
            MainEmotionUpdated?.Invoke();
        }
    }

    private void AddToEmotion(EmotionAnalysis.Emotion emot, float val)
    {

        if (!currentEmotions.ContainsKey(emot))
        {
            currentEmotions.Add(emot, Mathf.Clamp(val, 0, 100));
#if UNITY_EDITOR
            debugEmotions.Add(emot, Mathf.Clamp(val, 0, 100));
#endif
        }
        if (val == 0) return;
        currentEmotions[emot] += Mathf.Clamp((val + Random.Range(-0.002f, 0.002f)), 0, 100);
#if UNITY_EDITOR
        debugEmotions[emot] += Mathf.Clamp((val + Random.Range(-0.002f, 0.002f)), 0, 100);
#endif
    }
    //Passes the emotion the previous talker conveys to this character
    public void CalculateReaction(EmotionAnalysis emotionAnalysis)
    {
        EmotionAnalysis.Emotion reactionEmotion = MeasureEmotions(emotionAnalysis.score);
        float score = 0;
        if (reactionEmotion != EmotionAnalysis.Emotion.neutral) score = emotionAnalysis.score[reactionEmotion];

        switch (reactionEmotion)
        {
            case EmotionAnalysis.Emotion.neutral:

                break;
            case EmotionAnalysis.Emotion.fear:
                AddToEmotion(EmotionAnalysis.Emotion.negative, score);
                AddToEmotion(EmotionAnalysis.Emotion.fear, score);
                AddToEmotion(EmotionAnalysis.Emotion.anticipation, score);
                AddToEmotion(EmotionAnalysis.Emotion.sadness, score);
                break;
            case EmotionAnalysis.Emotion.anger:
                AddToEmotion(EmotionAnalysis.Emotion.anger, score);
                AddToEmotion(EmotionAnalysis.Emotion.fear, score);
                AddToEmotion(EmotionAnalysis.Emotion.negative, score);
                AddToEmotion(EmotionAnalysis.Emotion.sadness, score);
                break;
            case EmotionAnalysis.Emotion.anticipation:
                AddToEmotion(EmotionAnalysis.Emotion.neutral, score);
                AddToEmotion(EmotionAnalysis.Emotion.joy, score);
                AddToEmotion(EmotionAnalysis.Emotion.surprise, score);

                break;
            case EmotionAnalysis.Emotion.trust:
                AddToEmotion(EmotionAnalysis.Emotion.joy, score);
                AddToEmotion(EmotionAnalysis.Emotion.trust, score);
                AddToEmotion(EmotionAnalysis.Emotion.positive, score);
                break;
            case EmotionAnalysis.Emotion.surprise:
                AddToEmotion(EmotionAnalysis.Emotion.joy, score);
                AddToEmotion(EmotionAnalysis.Emotion.surprise, score);
                break;
            case EmotionAnalysis.Emotion.positive:
                AddToEmotion(EmotionAnalysis.Emotion.joy, score);
                AddToEmotion(EmotionAnalysis.Emotion.positive, score);
                break;
            case EmotionAnalysis.Emotion.negative:
                AddToEmotion(EmotionAnalysis.Emotion.negative, score);
                AddToEmotion(EmotionAnalysis.Emotion.anger, score);
                AddToEmotion(EmotionAnalysis.Emotion.fear, score);
                AddToEmotion(EmotionAnalysis.Emotion.sadness, score);
                break;
            case EmotionAnalysis.Emotion.sadness:
                AddToEmotion(EmotionAnalysis.Emotion.trust, score);
                AddToEmotion(EmotionAnalysis.Emotion.sadness, score);

                break;
            case EmotionAnalysis.Emotion.disgust:
                AddToEmotion(EmotionAnalysis.Emotion.sadness, score);
                AddToEmotion(EmotionAnalysis.Emotion.anger, score);
                AddToEmotion(EmotionAnalysis.Emotion.negative, score);
                break;
            case EmotionAnalysis.Emotion.joy:
                AddToEmotion(EmotionAnalysis.Emotion.joy, score);
                AddToEmotion(EmotionAnalysis.Emotion.anticipation, score);
                AddToEmotion(EmotionAnalysis.Emotion.positive, score);
                AddToEmotion(EmotionAnalysis.Emotion.surprise, score);
                break;

        }

        EmotionAnalysis.Emotion newEmotion = MeasureEmotions(currentEmotions);
        if (newEmotion != MainEmotion )
        {
            MainEmotion = newEmotion;
            if (!playParticleOnReaction)
            {
                skipReaction = true;
            }
            MainEmotionUpdated?.Invoke();
        }

    }

    private EmotionAnalysis.Emotion MeasureEmotions(Dictionary<EmotionAnalysis.Emotion, float> measure)
    {
        EmotionAnalysis.Emotion mainEmotion = EmotionAnalysis.Emotion.neutral;
        float emoteScore = 0;
        List<EmotionAnalysis.Emotion> equalEmotions = new List<EmotionAnalysis.Emotion>();
        foreach (KeyValuePair<EmotionAnalysis.Emotion, float> key in measure)
        {
            if (key.Value > emoteScore)
            {
                emoteScore = key.Value;
                mainEmotion = key.Key;
                equalEmotions.Clear();
                equalEmotions.Add(key.Key);
            }
            else if (key.Value == emoteScore)
            {
                equalEmotions.Add(key.Key);
            }
        }

        if (mainEmotion == EmotionAnalysis.Emotion.neutral) return EmotionAnalysis.Emotion.neutral;
        return equalEmotions[Random.Range(0, equalEmotions.Count)];
    }

    public void PlayCurrentEmotionParticle()
    {
        if(skipReaction)
        {
            skipReaction = false;
            return;
        }
        if (emotionParticles.ContainsKey(MainEmotion))
        {
            emotionParticles[MainEmotion].gameObject.SetActive(false);
            emotionParticles[MainEmotion].gameObject.SetActive(true);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIDirector : MonoBehaviour {
    private static AIDirector instance;
    public List<AICharacter> characters = new List<AICharacter>();

    public enum TokenType { GruntMelee,GruntRange,HeavyMelee,HeavyRange}
    [System.Serializable]
    public class TokenContainer
    {
        public TokenType tokenType;
        [Range(1,100)]
        public int maxTokens = 1;
        private int currentTokens = 0;

        public void RefilTokens()
        {
            currentTokens = maxTokens;
        }

        public bool TokenIsAvailable()
        {
            return currentTokens > 0;
        }

        public bool GetToken()
        {
            if (currentTokens == 0)
                return false;
            --currentTokens;
            return true;
        }

        public void ReturnToken()
        {
            currentTokens++;
            if (currentTokens > maxTokens)
                currentTokens = maxTokens;
        }
    }

    public TokenContainer[] tokens;
    
    void Awake()
    {
        if (instance)
            Destroy(this);
        instance = this;

        RefillTokens();
    }

    public void RefillTokens()
    {
        foreach (TokenContainer container in tokens)
            container.RefilTokens();
    }

    public static AIDirector GetInstance()
    {
        if (instance)
            return instance;

        instance = new GameObject("AI Director").AddComponent<AIDirector>();
        instance.RefillTokens();
        return instance;
    }


    public void Register(AICharacter character)
    {
        characters.Add(character);
    }

    public void Unregister(AICharacter character)
    {
        characters.Remove(character);
    }

    public bool RequestAttackToken(AICharacter character)
    {
        TokenContainer container = (from token in tokens
                                    where token.tokenType == character.stats.attackTokenType
                                    select token).First();

        if (!container.TokenIsAvailable())
            return false;

        var aiWithoutTokens = from ai in characters
                              where ai.stats.attackTokenType == character.stats.attackTokenType 
                              &&!ai.HasAttackToken 
                              && ai.IsAlert
                              select ai;

        float minDistance = aiWithoutTokens.Min(x => x.DistanceToPlayer);

        if(character.DistanceToPlayer == minDistance)
        {
            container.GetToken();
            return true;
        }
        return false;
    }

    public void ReturnAttackToken(AICharacter character)
    {
        TokenContainer container = (from token in tokens
                                    where token.tokenType == character.stats.attackTokenType
                                    select token).First();
        container.ReturnToken();
    }

    public void Alarm(Vector3 position,Vector3 playerPosition)
    {
        foreach (AICharacter ai in characters)
            if (!ai.IsAlert && Vector3.Distance(position, ai.Position) <= ai.stats.visibilityDistance
                && Utility.IsVisible(position, ai.gameObject, ai.stats.visibilityDistance,ai.Position))
                ai.Alarm(playerPosition);
                
    }

    public IEnumerable<AICharacter> GetClosestAI(Vector3 position, float distance)
    {
        //ClearDestroyedObjects();

        return from ai in characters
               where Vector3.Distance(position, ai.Position) <= distance
               select ai;
    }

    public int GetAICount()
    {
        return characters.Count;
    }
}

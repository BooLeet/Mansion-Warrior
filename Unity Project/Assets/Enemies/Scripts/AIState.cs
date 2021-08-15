using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    public abstract void Init(AICharacter character);
    public abstract void Action(AICharacter character);
    public abstract AIState Transition(AICharacter character);
}

namespace AIStates
{
    public class PatrolState : AIState
    {
        public override void Action(AICharacter character)
        {
            if (character.PatrolCheckDamageDirection)
                character.LookAt(character.LastDamageSource, Time.deltaTime * 5);
        }

        public override void Init(AICharacter character)
        {
            character.StopMovement();
            character.animator.Idle();
        }

        public override AIState Transition(AICharacter character)
        {
            if (character.PlayerIsDead)
                return null;

            if (character.PlayerIsInfront || character.IsAlert)
            {
                character.PatrolCheckDamageDirection = false;
                return new AlarmState();
            }
                
            return null;
        }
    }

    public class AlarmState : AIState
    {
        float timeCounter;

        public override void Action(AICharacter character)
        {
            timeCounter -= Time.deltaTime;
            character.LookAtPlayer(Time.deltaTime * 5);
        }

        public override void Init(AICharacter character)
        {
            character.Alarm(character.LastSeenPlayerPosition);
            character.animator.Alarm();
            timeCounter = character.stats.alarmDelay;
        }

        public override AIState Transition(AICharacter character)
        {
            if (timeCounter <= 0) 
                return new AlertState();
            return null;
        }
    }

    public class AlertState : AIState
    {
        public override void Action(AICharacter character)
        {
            character.LookAtPlayer(Time.deltaTime * 10);
        }

        public override void Init(AICharacter character)
        {
            character.StopMovement();
            character.animator.Idle();
        }

        public override AIState Transition(AICharacter character)
        {
            if (!character.PlayerIsVisible)
                return new FindPlayerState();

            if (character.PlayerIsDead)
                return new PatrolState();

            character.RequestAttackToken();
            if (character.HasAttackToken && character.CanAttack)
                return new AproachState();

            return null;
        }
    }

    public class FindPlayerState : AIState
    {
        float timeCounter = 0;
        private float secondsVisible = 0;
        private float maxSecondsVisible = 0.33f;

        public override void Action(AICharacter character)
        {
            character.Follow(character.LastSeenPlayerPosition, 0, true);
            if (character.PlayerIsVisible)
                secondsVisible += Time.deltaTime;
            else
                secondsVisible = 0;
            timeCounter += Time.deltaTime;
        }

        public override void Init(AICharacter character)
        {
            character.animator.Walk();
        }

        public override AIState Transition(AICharacter character)
        {
            if (character.PlayerIsDead)
                return new PatrolState();
            if (secondsVisible >= maxSecondsVisible) 
                return new AlertState();
            else if (timeCounter >= character.stats.cantFindTimeThreshold)
                return new PatrolState();
            return null;
        }
    }


    public class AproachState : AIState
    {
        float timeCounter = 0;

        public override void Action(AICharacter character)
        {
            if (character.PlayerIsVisible)
            {
                timeCounter = 0;
            }
            else
                timeCounter += Time.deltaTime;
            character.Follow(character.LastSeenPlayerPosition, 0, true);
        }

        public override void Init(AICharacter character)
        {
            character.animator.Walk();
        }

        public override AIState Transition(AICharacter character)
        {
            if (character.PlayerIsDead)
                return new PatrolState();

            if (!character.PlayerIsVisible && timeCounter >= character.stats.cantFindTimeThreshold)
            {
                character.ReturnAttackToken();
                return new PatrolState();
            }

            if (character.PlayerIsVisible && character.DistanceToPlayer <= character.stats.attackDistance)
            {
                character.StopMovement();
                return new AttackState();
            }

            return null;
        }
    }

    public class AttackState : AIState
    {
        public override void Action(AICharacter character)
        {
            if(character.AttackOnCooldown && character.CanAttack)
                character.LookAtPlayer(2 * Time.deltaTime);
        }

        public override void Init(AICharacter character)
        {
            // Attack
            character.LookAtPlayer();
            character.Attack();
        }

        public override AIState Transition(AICharacter character)
        {
            if (character.CanAttack && !character.AttackOnCooldown)
                return new AlertState();
            return null;
        }
    }

}
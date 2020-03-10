﻿using ME.ECS;

namespace Warcraft.Features {
    
    using TState = WarcraftState;
    
    public class UnitsAttackFeature : Feature<TState> {

        protected override void OnConstruct(ref ConstructParameters parameters) {

            this.AddSystem<Warcraft.Systems.UnitsAttackSystem>();

        }

        protected override void OnDeconstruct() {
            
        }

    }

}
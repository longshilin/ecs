﻿#if GAMEOBJECT_VIEWS_MODULE_SUPPORT
using EntityId = System.Int32;
using ViewId = System.UInt64;
using Tick = System.UInt64;

namespace ME.ECS {

    public partial interface IWorld<TState> where TState : class, IState<TState> {

        ViewId RegisterViewSource<TEntity>(UnityEngine.GameObject prefab) where TEntity : struct, IEntity;
        ViewId RegisterViewSource<TEntity, TProvider>(UnityEngine.GameObject prefab) where TEntity : struct, IEntity where TProvider : struct, IViewsProvider;
        void InstantiateView<TEntity>(UnityEngine.GameObject prefab, Entity entity) where TEntity : struct, IEntity;

    }

    public partial class World<TState> where TState : class, IState<TState>, new() {

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ViewId RegisterViewSource<TEntity>(UnityEngine.GameObject prefab) where TEntity : struct, IEntity {

            return this.RegisterViewSource<TEntity, UnityGameObjectProvider>(prefab);

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ViewId RegisterViewSource<TEntity, TProvider>(UnityEngine.GameObject prefab) where TEntity : struct, IEntity where TProvider : struct, IViewsProvider {

            IView<TEntity> component;
            if (prefab.TryGetComponent(out component) == true) {

                return this.RegisterViewSource<TEntity, TProvider>(component);

            }
            
            return 0UL;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void InstantiateView<TEntity>(UnityEngine.GameObject prefab, Entity entity) where TEntity : struct, IEntity {

            IView<TEntity> component;
            if (prefab.TryGetComponent(out component) == true) {

                this.InstantiateView(component, entity);

            }
            
        }

    }

    public abstract class MonoBehaviourView<TEntity> : UnityEngine.MonoBehaviour, IView<TEntity> where TEntity : struct, IEntity {

        public Entity entity { get; set; }
        public ViewId prefabSourceId { get; set; }

        public virtual void OnInitialize(in TEntity data) { }
        public virtual void OnDeInitialize(in TEntity data) { }
        public abstract void ApplyState(in TEntity data, float deltaTime, bool immediately);

    }
    
    public partial interface IViewModule<TState, TEntity> where TState : class, IState<TState> where TEntity : struct, IEntity {

        ViewId RegisterViewSource<TProvider>(UnityEngine.GameObject prefab) where TProvider : struct, IViewsProvider;
        void InstantiateView(UnityEngine.GameObject prefab, Entity entity);

    }

    public partial class ViewsModule<TState, TEntity> where TState : class, IState<TState> where TEntity : struct, IEntity {
        
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public ViewId RegisterViewSource<TProvider>(UnityEngine.GameObject prefab) where TProvider : struct, IViewsProvider {

            return this.RegisterViewSource<TProvider>(prefab.GetComponent<MonoBehaviourView<TEntity>>());

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void InstantiateView(UnityEngine.GameObject prefab, Entity entity) {
            
            var viewSource = prefab.GetComponent<MonoBehaviourView<TEntity>>();
            this.InstantiateView(this.GetViewSourceId(viewSource), entity);
            
        }

    }

    public class UnityGameObjectProvider<TEntity> : ViewsProvider<TEntity> where TEntity : struct, IEntity {

        public override IView<TEntity> Spawn(IView<TEntity> prefab, ViewId prefabSourceId) {

            return PoolGameObject.Spawn((MonoBehaviourView<TEntity>)prefab, prefabSourceId);

        }

        public override void Destroy(ref IView<TEntity> instance) {

            var instanceTyped = (MonoBehaviourView<TEntity>)instance;
            PoolGameObject.Recycle(ref instanceTyped);
            instance = null;

        }

    }

    public struct UnityGameObjectProvider : IViewsProvider {

        public IViewsProvider<TEntity> Create<TEntity>() where TEntity : struct, IEntity {

            return PoolClass<UnityGameObjectProvider<TEntity>>.Spawn();

        }

        public void Destroy<TEntity>(IViewsProvider<TEntity> instance) where TEntity : struct, IEntity {

            PoolClass<UnityGameObjectProvider<TEntity>>.Recycle((UnityGameObjectProvider<TEntity>)instance);
            
        }

    }

}
#endif
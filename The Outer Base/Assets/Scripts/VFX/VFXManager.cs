using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    private WaitForSeconds twoSeconds;
    [SerializeField] private GameObject reapingPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;
    
    [Header("Leaves Falling Prefab")]
    [SerializeField] private GameObject eoliaLeavesFallingPrefab = null;
    [SerializeField] private GameObject viliaLeavesFallingPrefab = null;
    [SerializeField] private GameObject obionyLeavesFallingPrefab = null;
    [SerializeField] private GameObject sillesLeavesFallingPrefab = null;
    [SerializeField] private GameObject gossiaLeavesFallingPrefab = null;

    [Header("Breaking Stone Prefab")]
    [SerializeField] private GameObject goraBreakingStonePrefab = null;
    [SerializeField] private GameObject berusBreakingStonePrefab = null;
    [SerializeField] private GameObject leonBreakingStonePrefab = null;
    [SerializeField] private GameObject larsBreakingStonePrefab = null;

    protected override void Awake()
    {
        base.Awake();
        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= displayHarvestActionEffect;
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += displayHarvestActionEffect;
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    private void displayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;
            case HarvestActionEffect.choppingTreeTrunk:
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;

            #region Leaves Falling Effect
            case HarvestActionEffect.eoliaLeavesFalling:
                GameObject eoliaLeavesFalling = PoolManager.Instance.ReuseObject(eoliaLeavesFallingPrefab, effectPosition, Quaternion.identity);
                eoliaLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(eoliaLeavesFalling, twoSeconds));
                break;
            case HarvestActionEffect.viliaLeavesFalling:
                GameObject viliaLeavesFalling = PoolManager.Instance.ReuseObject(viliaLeavesFallingPrefab, effectPosition, Quaternion.identity);
                viliaLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(viliaLeavesFalling, twoSeconds));
                break;
            case HarvestActionEffect.obionyLeavesFalling:
                GameObject obionyLeavesFalling = PoolManager.Instance.ReuseObject(obionyLeavesFallingPrefab, effectPosition, Quaternion.identity);
                obionyLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(obionyLeavesFalling, twoSeconds));
                break;
            case HarvestActionEffect.sillesLeavesFalling:
                GameObject sillesLeavesFalling = PoolManager.Instance.ReuseObject(sillesLeavesFallingPrefab, effectPosition, Quaternion.identity);
                sillesLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(sillesLeavesFalling, twoSeconds));
                break;
            case HarvestActionEffect.gossiaLeavesFalling:
                GameObject gossiaLeavesFalling = PoolManager.Instance.ReuseObject(gossiaLeavesFallingPrefab, effectPosition, Quaternion.identity);
                gossiaLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(gossiaLeavesFalling, twoSeconds));
                break;
            #endregion

            #region Stone Breaking Effect
            case HarvestActionEffect.goraBreakingStone:
                GameObject goraBreakingStone = PoolManager.Instance.ReuseObject(goraBreakingStonePrefab, effectPosition, Quaternion.identity);
                goraBreakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(goraBreakingStone, twoSeconds));
                break;
            case HarvestActionEffect.berusBreakingStone:
                GameObject berusBreakingStone = PoolManager.Instance.ReuseObject(berusBreakingStonePrefab, effectPosition, Quaternion.identity);
                berusBreakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(berusBreakingStone, twoSeconds));
                break;
            case HarvestActionEffect.leonBreakingStone:
                GameObject leonBreakingStone = PoolManager.Instance.ReuseObject(leonBreakingStonePrefab, effectPosition, Quaternion.identity);
                leonBreakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(leonBreakingStone, twoSeconds));
                break;
            case HarvestActionEffect.larsBreakingStone:
                GameObject larsBreakingStone = PoolManager.Instance.ReuseObject(larsBreakingStonePrefab, effectPosition, Quaternion.identity);
                larsBreakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(larsBreakingStone, twoSeconds));
                break;
            #endregion

            case HarvestActionEffect.none:
                break;
            default:
                break;
        }
    }
}

using UnityEngine;
using VoxelDungeon.Core;
using VoxelDungeon.Items;

namespace VoxelDungeon.Player
{
    public sealed class LoadoutVisualController : MonoBehaviour
    {
        private string lastMeleeId;
        private string lastRangedId;
        private string lastHeadId;
        private string lastBodyId;
        private string lastBootsId;
        private string lastAccessoryId;
        private Transform visualRoot;
        private float nextRefresh;

        private void Start()
        {
            HideBakedWeaponParts();
            ResolveVisualRoot();
            Refresh(true);
        }

        private void HideBakedWeaponParts()
        {
            string[] names =
            {
                "PlayerVisual/SwordBlade",
                "PlayerVisual/SwordGuard",
                "PlayerVisual/SwordGrip"
            };

            foreach (string path in names)
            {
                Transform part = transform.Find(path);
                if (part != null)
                    part.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (Time.time < nextRefresh)
                return;

            nextRefresh = Time.time + 0.25f;
            Refresh(false);
        }

        private void ResolveVisualRoot()
        {
            Transform existing = transform.Find("LoadoutVisual");
            if (existing != null)
            {
                visualRoot = existing;
                return;
            }

            GameObject go = new GameObject("LoadoutVisual");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            visualRoot = go.transform;
        }

        private void Refresh(bool force)
        {
            string meleeId = ProfileProgress.EquippedMeleeId;
            string rangedId = ProfileProgress.EquippedRangedId;
            string headId = ProfileProgress.EquippedHeadId;
            string bodyId = ProfileProgress.EquippedBodyId;
            string bootsId = ProfileProgress.EquippedBootsId;
            string accessoryId = ProfileProgress.EquippedAccessoryId;

            if (!force &&
                meleeId == lastMeleeId &&
                rangedId == lastRangedId &&
                headId == lastHeadId &&
                bodyId == lastBodyId &&
                bootsId == lastBootsId &&
                accessoryId == lastAccessoryId)
                return;

            lastMeleeId = meleeId;
            lastRangedId = rangedId;
            lastHeadId = headId;
            lastBodyId = bodyId;
            lastBootsId = bootsId;
            lastAccessoryId = accessoryId;

            ResolveVisualRoot();

            Transform hairTop = transform.Find("PlayerVisual/HairTop");
            if (hairTop != null)
                hairTop.gameObject.SetActive(string.IsNullOrEmpty(headId));

            for (int i = visualRoot.childCount - 1; i >= 0; i--)
                Destroy(visualRoot.GetChild(i).gameObject);

            BuildArmor(headId, bodyId, bootsId, accessoryId);
            BuildMelee(meleeId);
            BuildRanged(rangedId);
        }

        private void BuildArmor(string headId, string bodyId, string bootsId, string accessoryId)
        {
            Color headColor = EquipmentCatalog.GetRarityColor(EquipmentCatalog.Get(headId).Rarity);
            Color bodyColor = EquipmentCatalog.GetRarityColor(EquipmentCatalog.Get(bodyId).Rarity);
            Color bootsColor = EquipmentCatalog.GetRarityColor(EquipmentCatalog.Get(bootsId).Rarity);
            Color accessoryColor = EquipmentCatalog.GetRarityColor(EquipmentCatalog.Get(accessoryId).Rarity);

            CreateCube(
                "ArmorHead",
                new Vector3(0f, 1.98f, -0.02f),
                headId == "frontier_cap"
                    ? new Vector3(0.68f, 0.16f, 0.68f)
                    : new Vector3(0.72f, 0.24f, 0.72f),
                headColor);

            CreateCube(
                "ArmorChest",
                new Vector3(0f, 1.02f, 0.325f),
                bodyId == "frontier_vest"
                    ? new Vector3(0.58f, 0.42f, 0.055f)
                    : new Vector3(0.72f, 0.55f, 0.075f),
                bodyColor);

            CreateCube(
                "ArmorBoot_L",
                new Vector3(-0.22f, 0.13f, 0.10f),
                new Vector3(0.33f, 0.22f, 0.48f),
                bootsColor);

            CreateCube(
                "ArmorBoot_R",
                new Vector3(0.22f, 0.13f, 0.10f),
                new Vector3(0.33f, 0.22f, 0.48f),
                bootsColor);

            GameObject charm = CreateCube(
                "AccessoryCharm",
                new Vector3(-0.25f, 0.86f, 0.39f),
                new Vector3(0.14f, 0.22f, 0.08f),
                accessoryColor);

            charm.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
        }

        private void BuildMelee(string id)
        {
            Color metal = new Color(0.78f, 0.88f, 0.92f);
            Color accent = new Color(0.20f, 0.82f, 0.92f);
            Vector3 bladeScale = new Vector3(0.12f, 0.92f, 0.10f);
            Vector3 bladePos = new Vector3(0.72f, 1.06f, 0.30f);
            Vector3 headScale = Vector3.zero;
            Vector3 headPos = Vector3.zero;

            switch (id)
            {
                case "crystal_saber":
                    accent = new Color(0.14f, 0.92f, 1f);
                    metal = new Color(0.62f, 0.94f, 1f);
                    bladeScale = new Vector3(0.10f, 1.06f, 0.08f);
                    break;

                case "crystal_daggers":
                    accent = new Color(0.20f, 0.96f, 1f);
                    metal = new Color(0.72f, 0.96f, 1f);
                    bladeScale = new Vector3(0.09f, 0.66f, 0.07f);
                    bladePos = new Vector3(0.70f, 0.93f, 0.30f);
                    break;

                case "warden_cleaver":
                    accent = new Color(0.62f, 0.30f, 1f);
                    metal = new Color(0.44f, 0.48f, 0.58f);
                    bladeScale = new Vector3(0.28f, 1.12f, 0.12f);
                    break;

                case "ember_axe":
                    accent = new Color(1f, 0.42f, 0.08f);
                    metal = new Color(0.42f, 0.44f, 0.46f);
                    bladeScale = new Vector3(0.10f, 0.88f, 0.10f);
                    headScale = new Vector3(0.46f, 0.32f, 0.12f);
                    headPos = new Vector3(0.88f, 1.42f, 0.30f);
                    break;

                case "forge_spear":
                    accent = new Color(1f, 0.52f, 0.08f);
                    metal = new Color(0.54f, 0.56f, 0.58f);
                    bladeScale = new Vector3(0.08f, 1.55f, 0.08f);
                    bladePos = new Vector3(0.73f, 1.12f, 0.30f);
                    headScale = new Vector3(0.22f, 0.40f, 0.10f);
                    headPos = new Vector3(0.92f, 1.84f, 0.30f);
                    break;

                case "colossus_maul":
                    accent = new Color(1f, 0.24f, 0.04f);
                    metal = new Color(0.30f, 0.31f, 0.34f);
                    bladeScale = new Vector3(0.12f, 1.08f, 0.12f);
                    headScale = new Vector3(0.62f, 0.42f, 0.42f);
                    headPos = new Vector3(0.86f, 1.48f, 0.30f);
                    break;

                case "moonblade":
                    accent = new Color(0.72f, 0.54f, 1f);
                    metal = new Color(0.88f, 0.92f, 1f);
                    bladeScale = new Vector3(0.09f, 1.18f, 0.08f);
                    break;

                case "void_edge":
                    accent = new Color(0.22f, 0.92f, 0.58f);
                    metal = new Color(0.18f, 0.20f, 0.25f);
                    bladeScale = new Vector3(0.18f, 1.18f, 0.10f);
                    break;
            }

            GameObject blade = CreateCube("MeleeBlade", bladePos, bladeScale, metal);
            blade.transform.localRotation = Quaternion.Euler(-18f, 0f, -18f);

            CreateCube(
                "MeleeGrip",
                new Vector3(0.52f, 0.54f, 0.20f),
                new Vector3(0.14f, 0.28f, 0.14f),
                new Color(0.24f, 0.12f, 0.06f));

            CreateCube(
                "MeleeGuard",
                new Vector3(0.58f, 0.69f, 0.24f),
                new Vector3(0.34f, 0.10f, 0.12f),
                accent);

            if (headScale != Vector3.zero)
                CreateCube("MeleeHead", headPos, headScale, metal);
        }

        private void BuildRanged(string id)
        {
            Color body = id switch
            {
                "crystal_bow" => new Color(0.18f, 0.72f, 1f),
                "ember_repeater" => new Color(1f, 0.40f, 0.08f),
                "void_staff" => new Color(0.62f, 0.30f, 1f),
                "starbow" => new Color(0.24f, 0.90f, 0.62f),
                "astral_crossbow" => new Color(0.72f, 0.62f, 1f),
                _ => new Color(0.48f, 0.30f, 0.16f)
            };

            if (id == "void_staff")
            {
                CreateCube(
                    "BackStaff",
                    new Vector3(-0.48f, 1.00f, -0.34f),
                    new Vector3(0.10f, 1.20f, 0.10f),
                    new Color(0.30f, 0.22f, 0.40f));

                CreateCube(
                    "StaffCore",
                    new Vector3(-0.48f, 1.62f, -0.34f),
                    new Vector3(0.28f, 0.28f, 0.28f),
                    body);
                return;
            }

            GameObject bow = CreateCube(
                "BackRanged",
                new Vector3(-0.44f, 1.05f, -0.36f),
                new Vector3(0.12f, 0.95f, 0.12f),
                body);

            bow.transform.localRotation = Quaternion.Euler(10f, 0f, 24f);

            CreateCube(
                "RangedCore",
                new Vector3(-0.34f, 1.05f, -0.40f),
                new Vector3(0.24f, 0.20f, 0.12f),
                body);
        }

        private GameObject CreateCube(string name, Vector3 localPosition, Vector3 scale, Color color)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(visualRoot, false);
            go.transform.localPosition = localPosition;
            go.transform.localScale = scale;

            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = color;

            return go;
        }
    }
}

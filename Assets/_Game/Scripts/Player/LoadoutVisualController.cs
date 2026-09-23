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

            bool fullHeadCover = headId == "crystal_hood" || headId == "forge_helm";

            Transform hairTop = transform.Find("PlayerVisual/HairTop");
            if (hairTop != null)
                hairTop.gameObject.SetActive(string.IsNullOrEmpty(headId) || headId == "astral_crown");

            string[] hairDetails =
            {
                "PlayerVisual/HairSide_L",
                "PlayerVisual/HairSide_R",
                "PlayerVisual/HairFront_L",
                "PlayerVisual/HairFront_R"
            };

            foreach (string hairPath in hairDetails)
            {
                Transform hairPart = transform.Find(hairPath);
                if (hairPart != null)
                    hairPart.gameObject.SetActive(!fullHeadCover);
            }

            Transform bakedChest = transform.Find("PlayerVisual/ChestPanel");
            Transform bakedChestTrim = transform.Find("PlayerVisual/ChestTrim");
            bool showBakedChest = string.IsNullOrEmpty(bodyId);
            if (bakedChest != null) bakedChest.gameObject.SetActive(showBakedChest);
            if (bakedChestTrim != null) bakedChestTrim.gameObject.SetActive(showBakedChest);

            Transform bakedBootL = transform.Find("PlayerVisual/Boot_L");
            Transform bakedBootR = transform.Find("PlayerVisual/Boot_R");
            bool showBakedBoots = string.IsNullOrEmpty(bootsId);
            if (bakedBootL != null) bakedBootL.gameObject.SetActive(showBakedBoots);
            if (bakedBootR != null) bakedBootR.gameObject.SetActive(showBakedBoots);

            for (int i = visualRoot.childCount - 1; i >= 0; i--)
                Destroy(visualRoot.GetChild(i).gameObject);

            BuildArmor(headId, bodyId, bootsId, accessoryId);
            BuildMelee(meleeId);
            BuildRanged(rangedId);
        }

        private void BuildArmor(string headId, string bodyId, string bootsId, string accessoryId)
        {
            Color headColor = GetEquipmentVisualColor(headId);
            Color bodyColor = GetEquipmentVisualColor(bodyId);
            Color bootsColor = GetEquipmentVisualColor(bootsId);
            Color accessoryColor = EquipmentCatalog.GetRarityColor(EquipmentCatalog.Get(accessoryId).Rarity);
            Color accent = Color.Lerp(bodyColor, Color.white, 0.34f);

            if (!string.IsNullOrEmpty(headId))
            {
                if (headId == "frontier_cap")
                {
                    CreateCube(
                        "ArmorHead",
                        new Vector3(0f, 1.965f, -0.025f),
                        new Vector3(0.61f, 0.12f, 0.60f),
                        headColor);

                    CreateCube(
                        "ArmorBrim",
                        new Vector3(0f, 1.90f, 0.27f),
                        new Vector3(0.44f, 0.055f, 0.17f),
                        Color.Lerp(headColor, Color.black, 0.16f));
                }
                else if (headId == "astral_crown")
                {
                    CreateCube(
                        "ArmorHead",
                        new Vector3(0f, 2.075f, -0.01f),
                        new Vector3(0.60f, 0.065f, 0.58f),
                        headColor);

                    CreateCube(
                        "CrownGem",
                        new Vector3(0f, 2.17f, 0.17f),
                        new Vector3(0.16f, 0.20f, 0.12f),
                        new Color(0.36f, 0.88f, 1f));
                }
                else
                {
                    CreateCube(
                        "ArmorHead",
                        new Vector3(0f, 1.91f, -0.015f),
                        new Vector3(0.62f, 0.19f, 0.60f),
                        headColor);

                    CreateCube(
                        "HelmetBand",
                        new Vector3(0f, 1.86f, 0.29f),
                        new Vector3(0.48f, 0.075f, 0.055f),
                        accent);
                }
            }

            if (!string.IsNullOrEmpty(bodyId))
            {
                CreateCube(
                    "ArmorChest",
                    new Vector3(0f, 1.045f, 0.292f),
                    bodyId == "frontier_vest"
                        ? new Vector3(0.48f, 0.34f, 0.055f)
                        : new Vector3(0.56f, 0.42f, 0.075f),
                    bodyColor);

                CreateCube(
                    "ArmorChestTrim",
                    new Vector3(0f, 1.16f, 0.337f),
                    new Vector3(0.42f, 0.065f, 0.035f),
                    accent);

                if (bodyId == "ember_plate" || bodyId == "void_mantle")
                {
                    CreateCube(
                        "ArmorSide_L",
                        new Vector3(-0.33f, 1.02f, 0.24f),
                        new Vector3(0.11f, 0.34f, 0.10f),
                        Color.Lerp(bodyColor, Color.black, 0.12f));

                    CreateCube(
                        "ArmorSide_R",
                        new Vector3(0.33f, 1.02f, 0.24f),
                        new Vector3(0.11f, 0.34f, 0.10f),
                        Color.Lerp(bodyColor, Color.black, 0.12f));
                }
            }

            if (!string.IsNullOrEmpty(bootsId))
            {
                CreateCube(
                    "ArmorBoot_L",
                    new Vector3(-0.19f, 0.13f, 0.09f),
                    new Vector3(0.30f, 0.21f, 0.44f),
                    bootsColor);

                CreateCube(
                    "ArmorBoot_R",
                    new Vector3(0.19f, 0.13f, 0.09f),
                    new Vector3(0.30f, 0.21f, 0.44f),
                    bootsColor);
            }

            if (!string.IsNullOrEmpty(accessoryId))
            {
                GameObject charm = CreateCube(
                    "AccessoryCharm",
                    new Vector3(-0.24f, 0.86f, 0.34f),
                    new Vector3(0.12f, 0.18f, 0.075f),
                    accessoryColor);

                charm.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
            }
        }

        private static Color GetEquipmentVisualColor(string id)
        {
            return id switch
            {
                "frontier_cap" => new Color(0.10f, 0.16f, 0.20f),
                "crystal_hood" => new Color(0.10f, 0.42f, 0.50f),
                "forge_helm" => new Color(0.25f, 0.27f, 0.29f),
                "astral_crown" => new Color(0.66f, 0.70f, 0.78f),

                "frontier_vest" => new Color(0.12f, 0.32f, 0.38f),
                "crystal_mail" => new Color(0.18f, 0.50f, 0.56f),
                "ember_plate" => new Color(0.34f, 0.27f, 0.22f),
                "void_mantle" => new Color(0.24f, 0.20f, 0.34f),

                "trail_boots" => new Color(0.25f, 0.14f, 0.08f),
                "crystal_steps" => new Color(0.14f, 0.38f, 0.44f),
                "ember_greaves" => new Color(0.28f, 0.25f, 0.23f),
                "moonstep_boots" => new Color(0.48f, 0.48f, 0.62f),

                _ => new Color(0.32f, 0.36f, 0.40f)
            };
        }

        private void BuildMelee(string id)
        {
            Transform meleeRig = CreateRig("MeleeRig");
            meleeRig.localPosition = new Vector3(0.49f, 0.54f, 0.10f);
            meleeRig.localRotation = Quaternion.Euler(0f, 0f, -8f);

            Color metal = new Color(0.76f, 0.86f, 0.90f);
            Color accent = new Color(0.20f, 0.82f, 0.92f);
            Color gripColor = new Color(0.22f, 0.11f, 0.055f);

            float bladeLength = 0.86f;
            float bladeWidth = 0.14f;
            float bladeDepth = 0.16f;
            float gripLength = 0.26f;
            float guardWidth = 0.34f;
            bool hasHead = false;
            Vector3 headScale = Vector3.zero;
            float headY = 1.04f;

            switch (id)
            {
                case "crystal_saber":
                    accent = new Color(0.14f, 0.92f, 1f);
                    metal = new Color(0.62f, 0.94f, 1f);
                    bladeLength = 1.02f;
                    bladeWidth = 0.11f;
                    bladeDepth = 0.14f;
                    break;

                case "crystal_daggers":
                    accent = new Color(0.20f, 0.96f, 1f);
                    metal = new Color(0.72f, 0.96f, 1f);
                    bladeLength = 0.58f;
                    bladeWidth = 0.10f;
                    guardWidth = 0.24f;
                    gripLength = 0.20f;
                    break;

                case "warden_cleaver":
                    accent = new Color(0.62f, 0.30f, 1f);
                    metal = new Color(0.44f, 0.48f, 0.58f);
                    bladeLength = 1.00f;
                    bladeWidth = 0.28f;
                    bladeDepth = 0.18f;
                    break;

                case "ember_axe":
                    accent = new Color(1f, 0.42f, 0.08f);
                    metal = new Color(0.42f, 0.44f, 0.46f);
                    bladeLength = 0.82f;
                    bladeWidth = 0.095f;
                    hasHead = true;
                    headScale = new Vector3(0.48f, 0.30f, 0.18f);
                    headY = 1.02f;
                    break;

                case "forge_spear":
                    accent = new Color(1f, 0.52f, 0.08f);
                    metal = new Color(0.54f, 0.56f, 0.58f);
                    bladeLength = 1.42f;
                    bladeWidth = 0.08f;
                    bladeDepth = 0.10f;
                    hasHead = true;
                    headScale = new Vector3(0.20f, 0.34f, 0.13f);
                    headY = 1.54f;
                    break;

                case "colossus_maul":
                    accent = new Color(1f, 0.24f, 0.04f);
                    metal = new Color(0.30f, 0.31f, 0.34f);
                    bladeLength = 0.94f;
                    bladeWidth = 0.11f;
                    bladeDepth = 0.13f;
                    hasHead = true;
                    headScale = new Vector3(0.62f, 0.40f, 0.44f);
                    headY = 1.10f;
                    break;

                case "moonblade":
                    accent = new Color(0.72f, 0.54f, 1f);
                    metal = new Color(0.88f, 0.92f, 1f);
                    bladeLength = 1.10f;
                    bladeWidth = 0.10f;
                    bladeDepth = 0.14f;
                    break;

                case "void_edge":
                    accent = new Color(0.22f, 0.92f, 0.58f);
                    metal = new Color(0.18f, 0.20f, 0.25f);
                    bladeLength = 1.10f;
                    bladeWidth = 0.19f;
                    bladeDepth = 0.17f;
                    break;
            }

            // The weapon is built around the hand pivot instead of around the
            // player's origin. This keeps the blade upright and gives attack
            // animation a natural rotation point.
            CreateCube(
                meleeRig,
                "MeleeGrip",
                new Vector3(0f, gripLength * 0.5f, 0f),
                new Vector3(0.13f, gripLength, 0.14f),
                gripColor);

            CreateCube(
                meleeRig,
                "MeleeGuard",
                new Vector3(0f, gripLength + 0.05f, 0.02f),
                new Vector3(guardWidth, 0.10f, 0.15f),
                accent);

            float bladeCenterY = gripLength + 0.10f + bladeLength * 0.5f;
            GameObject blade = CreateCube(
                meleeRig,
                "MeleeBlade",
                new Vector3(0f, bladeCenterY, 0.035f),
                new Vector3(bladeWidth, bladeLength, bladeDepth),
                metal);

            blade.transform.localRotation = Quaternion.identity;

            if (hasHead)
            {
                CreateCube(
                    meleeRig,
                    "MeleeHead",
                    new Vector3(0f, headY, 0.035f),
                    headScale,
                    metal);
            }

            if (id == "crystal_daggers")
            {
                Transform offhand = CreateRig("OffhandMeleeRig");
                offhand.localPosition = new Vector3(-0.49f, 0.54f, 0.10f);
                offhand.localRotation = Quaternion.Euler(0f, 0f, 8f);

                CreateCube(offhand, "OffhandGrip", new Vector3(0f, 0.10f, 0f), new Vector3(0.11f, 0.20f, 0.12f), gripColor);
                CreateCube(offhand, "OffhandBlade", new Vector3(0f, 0.46f, 0.035f), new Vector3(0.10f, 0.56f, 0.14f), metal);
            }
        }

        private void BuildRanged(string id)
        {
            Transform rangedRig = CreateRig("RangedRig");

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
                CreateCube(rangedRig, 
                    "BackStaff",
                    new Vector3(-0.48f, 1.00f, -0.34f),
                    new Vector3(0.10f, 1.20f, 0.10f),
                    new Color(0.30f, 0.22f, 0.40f));

                CreateCube(rangedRig, 
                    "StaffCore",
                    new Vector3(-0.48f, 1.62f, -0.34f),
                    new Vector3(0.28f, 0.28f, 0.28f),
                    body);
                return;
            }

            GameObject bow = CreateCube(rangedRig, 
                "BackRanged",
                new Vector3(-0.44f, 1.05f, -0.36f),
                new Vector3(0.12f, 0.95f, 0.12f),
                body);

            bow.transform.localRotation = Quaternion.Euler(10f, 0f, 24f);

            CreateCube(rangedRig, 
                "RangedCore",
                new Vector3(-0.34f, 1.05f, -0.40f),
                new Vector3(0.24f, 0.20f, 0.12f),
                body);
        }

        private Transform CreateRig(string name)
        {
            GameObject rig = new GameObject(name);
            rig.transform.SetParent(visualRoot, false);
            rig.transform.localPosition = Vector3.zero;
            rig.transform.localRotation = Quaternion.identity;
            return rig.transform;
        }

        private GameObject CreateCube(string name, Vector3 localPosition, Vector3 scale, Color color)
        {
            return CreateCube(visualRoot, name, localPosition, scale, color);
        }

        private GameObject CreateCube(Transform parent, string name, Vector3 localPosition, Vector3 scale, Color color)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
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

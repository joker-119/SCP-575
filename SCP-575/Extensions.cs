namespace SCP_575
{
	public static class Extensions
	{
		public static bool HasLightSource(this ReferenceHub rh)
		{
			if (rh.inventory != null && rh.inventory.curItem == ItemType.Flashlight)
				return true;
			if (rh.inventory == null || rh.weaponManager == null || !rh.weaponManager.NetworksyncFlash ||
			    rh.weaponManager.curWeapon < 0 ||
			    rh.weaponManager.curWeapon >= rh.weaponManager.weapons.Length) return false;
			WeaponManager.Weapon weapon = rh.weaponManager.weapons[rh.weaponManager.curWeapon];
			Inventory.SyncItemInfo itemInHand = rh.inventory.GetItemInHand();
			if (weapon == null || itemInHand.modOther < 0 || itemInHand.modOther >= weapon.mod_others.Length)
				return false;
			WeaponManager.Weapon.WeaponMod modOther = weapon.mod_others[itemInHand.modOther];
			if (modOther != null && !string.IsNullOrEmpty(modOther.name) && (modOther.name.ToLower().Contains("flashlight") || modOther.name.Contains("night")))
				return true;
			return false;
		}
		
		public static void Broadcast(this ReferenceHub rh, uint time, string message) =>
			rh.GetComponent<Broadcast>()
				.TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);
	}
}
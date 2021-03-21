public interface ICharacterDamager
{
	/// <summary>
	/// Called when a character hits this object
	/// </summary>
	/// <param name="character">The character that hit this object</param>
	void OnHit(Character character);
}

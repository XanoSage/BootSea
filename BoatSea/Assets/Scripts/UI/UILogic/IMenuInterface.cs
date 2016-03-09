public interface IMenuInterface {
	UIMenuInterfaceControllsType MenuType { get; set; }
	bool IsActive { get; set; }
    void Show();
    void Hide();
}
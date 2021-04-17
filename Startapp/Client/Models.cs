namespace Startapp.Client
{
    public class ModalParams
    {
        public string Title { get; set; } 
        public string Message { get; set; } 
        public string Type { get; set; } 
        public string Icon { get; set; } 
        public string Button { get; set; }
    }
    public class ModalSuccess
    {        
        public string Title { get; set; } = "Modal success";
        public string Message { get; set; } = "Modal success";
        public string Type { get; set; } = "modal-success";
        public string Icon { get; set; } = "fas fa-check";
        public string Button { get; set; } = "btn-success";
    }
    public class ModalInfo 
    {
        public string Title { get; set; } = "Modal info";
        public string Message { get; set; } = "Modal info";
        public string Type { get; set; } = "modal-info";
        public string Icon { get; set; } = "fas fa-bell";
        public string Button { get; set; } = "btn-info";
    }
    public class ModalWarning
    {
        public string Title { get; set; } = "Modal warning";
        public string Message { get; set; } = "Modal warning";
        public string Type { get; set; } = "modal-warning";
        public string Icon { get; set; } = "fas fa-exclamation-triangle";
        public string Button { get; set; } = "btn-warning";
    }
    public class ModalError
    {
        public string Title { get; set; } = "Modal error";
        public string Message { get; set; } = "Modal error";
        public string Type { get; set; } = "modal-danger";
        public string Icon { get; set; } = "fas fa-ban";
        public string Button { get; set; } = "btn-danger";
    }
}

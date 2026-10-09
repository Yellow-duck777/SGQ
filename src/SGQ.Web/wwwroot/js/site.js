(() => {
  const body = document.body;
  const toggle = document.querySelector("[data-sidebar-toggle]");
  const close = document.querySelector("[data-sidebar-close]");

  const setSidebar = (open) => {
    body.classList.toggle("sidebar-open", open);
    toggle?.setAttribute("aria-expanded", String(open));
  };

  toggle?.addEventListener("click", () => setSidebar(!body.classList.contains("sidebar-open")));
  close?.addEventListener("click", () => setSidebar(false));
  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") setSidebar(false);
  });

  document.querySelectorAll("[data-password-toggle]").forEach((button) => {
    button.addEventListener("click", () => {
      const input = button.closest(".password-field")?.querySelector("input");
      if (!input) return;

      const show = input.type === "password";
      input.type = show ? "text" : "password";
      button.textContent = show ? "Ocultar" : "Mostrar";
      button.setAttribute("aria-label", show ? "Ocultar senha" : "Mostrar senha");
    });
  });

  document.querySelectorAll("[data-file-picker]").forEach((input) => {
    const nome = input.closest(".file-picker")?.querySelector("[data-file-picker-name]");
    input.addEventListener("change", () => {
      if (nome) nome.textContent = input.files && input.files.length ? input.files[0].name : "Nenhum arquivo selecionado · máximo de 25 MB";
    });
  });
})();

// Pede confirmação ao salvar um usuário sem nenhum perfil (a conta fica bloqueada).
(() => {
  document.querySelectorAll("form[data-usr-form]").forEach((form) => {
    const caixas = Array.from(form.querySelectorAll("input[type=checkbox][name=perfis]"));
    const atualizar = () => {
      if (caixas.some((c) => c.checked)) form.removeAttribute("data-confirm");
      else form.setAttribute("data-confirm", "Salvar sem nenhum perfil? A conta ficará bloqueada até que um perfil seja atribuído.");
    };
    caixas.forEach((c) => c.addEventListener("change", atualizar));
    atualizar();
  });
})();

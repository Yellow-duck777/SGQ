// Busca por texto nas listas de cadastros e preenchimento do ano a partir da data.
(() => {
  const normalizar = (texto) => texto.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");

  document.querySelectorAll("[data-cad-search]").forEach((campo) => {
    const alvo = document.querySelector(campo.dataset.cadSearch);
    if (!alvo) return;
    const linhas = Array.from(alvo.querySelectorAll("tbody tr[data-busca]"));
    const contador = document.querySelector("[data-cad-count]");
    const vazio = document.querySelector("[data-cad-empty]");
    const rotuloTotal = contador?.dataset.cadCount || "";

    const filtrar = () => {
      const termo = normalizar(campo.value.trim());
      let visiveis = 0;
      linhas.forEach((linha) => {
        const mostra = !termo || normalizar(linha.dataset.busca).includes(termo);
        linha.hidden = !mostra;
        if (mostra) visiveis++;
      });
      alvo.querySelectorAll("[data-cad-group]").forEach((grupo) => {
        grupo.hidden = !grupo.querySelector("tbody tr[data-busca]:not([hidden])");
      });
      if (vazio) vazio.hidden = visiveis !== 0 || linhas.length === 0;
      if (contador) contador.textContent = termo ? `${visiveis} de ${linhas.length} ${rotuloTotal}` : `${linhas.length} ${rotuloTotal}`;
    };
    campo.addEventListener("input", filtrar);
  });

  const data = document.querySelector("[data-cad-data]");
  const ano = document.querySelector("[data-cad-ano]");
  if (data && ano) {
    data.addEventListener("change", () => {
      const m = /^(\d{4})-/.exec(data.value);
      if (m) ano.value = m[1];
    });
  }
})();

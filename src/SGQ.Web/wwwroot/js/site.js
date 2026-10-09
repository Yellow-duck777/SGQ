(() => {
  "use strict";

  const body = document.body;
  const mobileQuery = window.matchMedia("(max-width: 900px)");
  const focusable = 'a[href], button:not([disabled]), input:not([disabled]):not([type="hidden"]), select:not([disabled]), textarea:not([disabled]), summary, [tabindex]:not([tabindex="-1"])';

  /* ---------- Menu lateral (gaveta no celular) ---------- */
  const sidebar = document.getElementById("app-sidebar");
  const sidebarToggle = document.querySelector("[data-sidebar-toggle]");

  const sidebarOpen = () => body.classList.contains("sidebar-open");
  const setSidebar = (open, devolverFoco = true) => {
    if (!sidebar) return;
    body.classList.toggle("sidebar-open", open);
    sidebarToggle?.setAttribute("aria-expanded", String(open));
    if (open) sidebar.querySelector(".sidebar-link")?.focus();
    else if (devolverFoco && mobileQuery.matches) sidebarToggle?.focus();
  };

  sidebarToggle?.addEventListener("click", () => setSidebar(!sidebarOpen()));
  document.querySelectorAll("[data-sidebar-close]").forEach((el) => el.addEventListener("click", () => setSidebar(false)));
  sidebar?.addEventListener("click", (event) => {
    if (mobileQuery.matches && event.target.closest("a.sidebar-link")) setSidebar(false, false);
  });
  mobileQuery.addEventListener("change", () => { if (!mobileQuery.matches) setSidebar(false, false); });

  /* ---------- Menu do usuário ---------- */
  const userToggle = document.querySelector("[data-user-menu-toggle]");
  const userMenu = document.querySelector("[data-user-menu]");
  const setUserMenu = (open, devolverFoco = false) => {
    if (!userToggle || !userMenu) return;
    userMenu.hidden = !open;
    userToggle.setAttribute("aria-expanded", String(open));
    if (open) userMenu.querySelector("a, button")?.focus();
    else if (devolverFoco) userToggle.focus();
  };
  userToggle?.addEventListener("click", () => setUserMenu(userMenu.hidden));
  document.addEventListener("click", (event) => {
    if (userMenu && !userMenu.hidden && !event.target.closest("[data-user-menu], [data-user-menu-toggle]")) setUserMenu(false);
  });
  userMenu?.addEventListener("focusout", (event) => {
    if (event.relatedTarget && !userMenu.contains(event.relatedTarget) && event.relatedTarget !== userToggle) setUserMenu(false);
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      if (userMenu && !userMenu.hidden) { setUserMenu(false, true); return; }
      if (sidebarOpen()) { setSidebar(false); return; }
    }
    // Foco preso na gaveta enquanto ela está aberta
    if (event.key === "Tab" && sidebarOpen() && mobileQuery.matches && sidebar) {
      const itens = [...sidebar.querySelectorAll(focusable)].filter((el) => el.offsetParent !== null);
      if (!itens.length) return;
      const primeiro = itens[0];
      const ultimo = itens[itens.length - 1];
      if (event.shiftKey && document.activeElement === primeiro) { event.preventDefault(); ultimo.focus(); }
      else if (!event.shiftKey && document.activeElement === ultimo) { event.preventDefault(); primeiro.focus(); }
      else if (!sidebar.contains(document.activeElement)) { event.preventDefault(); primeiro.focus(); }
    }
  });

  /* ---------- Campo de senha e seletor de arquivo ---------- */
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

  /* ---------- Avisos (toasts) ---------- */
  const regiao = document.querySelector("[data-aviso-region]");

  const fecharAviso = (aviso) => {
    if (!aviso || aviso.dataset.fechando) return;
    aviso.dataset.fechando = "1";
    aviso.classList.add("is-leaving");
    window.setTimeout(() => aviso.remove(), 180);
  };

  const prepararAviso = (aviso) => {
    aviso.querySelector("[data-aviso-close]")?.addEventListener("click", () => fecharAviso(aviso));
    if (aviso.dataset.tipo === "sucesso" || aviso.dataset.tipo === "info") {
      let timer = window.setTimeout(() => fecharAviso(aviso), 6000);
      const pausar = () => window.clearTimeout(timer);
      const retomar = () => { timer = window.setTimeout(() => fecharAviso(aviso), 3000); };
      aviso.addEventListener("mouseenter", pausar);
      aviso.addEventListener("mouseleave", retomar);
      aviso.addEventListener("focusin", pausar);
      aviso.addEventListener("focusout", retomar);
    }
  };

  const textoDe = (el) => (el.textContent || "").replace(/\s+/g, " ").trim();

  /** Mostra um aviso: tipo = sucesso | erro | alerta | info. */
  window.sgqAviso = (mensagem, tipo = "info") => {
    if (!regiao) return;
    const aviso = document.createElement("div");
    aviso.className = "aviso";
    aviso.dataset.tipo = tipo;
    aviso.setAttribute("role", tipo === "erro" ? "alert" : "status");
    const texto = document.createElement("p");
    texto.className = "aviso-text";
    texto.textContent = mensagem;
    const botao = document.createElement("button");
    botao.type = "button";
    botao.className = "aviso-close";
    botao.setAttribute("aria-label", "Fechar aviso");
    botao.setAttribute("data-aviso-close", "");
    botao.textContent = "×";
    aviso.append(texto, botao);
    regiao.append(aviso);
    prepararAviso(aviso);
  };

  if (regiao) {
    const avisos = [...regiao.querySelectorAll(".aviso")];
    avisos.forEach(prepararAviso);
    // As telas antigas ainda mostram o mesmo TempData em um alerta no corpo da página: evita a repetição.
    const textos = new Set(avisos.map((aviso) => textoDe(aviso.querySelector(".aviso-text") ?? aviso)));
    document.querySelectorAll("main .alert").forEach((alerta) => {
      if (textos.has(textoDe(alerta))) alerta.remove();
    });
  }

  /* ---------- Confirmação (data-confirm) ---------- */
  let dialogo = null;
  let aoDecidir = null;

  const criarDialogo = () => {
    const el = document.createElement("dialog");
    el.className = "confirm-dialog";
    el.setAttribute("aria-labelledby", "confirm-title");
    el.setAttribute("aria-describedby", "confirm-text");
    el.innerHTML =
      '<form method="dialog" class="confirm-box">' +
      '<h2 id="confirm-title">Confirmar ação</h2>' +
      '<p id="confirm-text"></p>' +
      '<div class="confirm-actions">' +
      '<button type="submit" value="cancelar" class="btn btn-outline-secondary" data-confirm-cancel>Cancelar</button>' +
      '<button type="submit" value="confirmar" class="btn btn-primary" data-confirm-ok>Confirmar</button>' +
      "</div></form>";
    el.addEventListener("close", () => {
      const confirmou = el.returnValue === "confirmar";
      el.returnValue = "";
      const callback = aoDecidir;
      aoDecidir = null;
      callback?.(confirmou);
    });
    // Clique fora da caixa cancela
    el.addEventListener("click", (event) => {
      if (event.target === el) el.close("cancelar");
    });
    document.body.append(el);
    return el;
  };

  const perguntar = (pergunta, destrutivo, callback) => {
    if (typeof HTMLDialogElement === "undefined" || typeof HTMLDialogElement.prototype.showModal !== "function") {
      callback(window.confirm(pergunta));
      return;
    }
    dialogo ??= criarDialogo();
    dialogo.querySelector("#confirm-text").textContent = pergunta;
    dialogo.querySelector("[data-confirm-ok]").className = "btn " + (destrutivo ? "btn-danger" : "btn-primary");
    aoDecidir = callback;
    dialogo.showModal();
    dialogo.querySelector("[data-confirm-cancel]")?.focus();
  };

  const confirmados = new WeakSet();

  document.addEventListener("submit", (event) => {
    const form = event.target;
    if (!(form instanceof HTMLFormElement) || confirmados.has(form)) return;
    const submitter = event.submitter;
    const pergunta = submitter?.getAttribute("data-confirm") ?? form.getAttribute("data-confirm");
    if (!pergunta) return;

    event.preventDefault();
    event.stopImmediatePropagation();
    const destrutivo = !(submitter ?? form).hasAttribute("data-confirm-neutral");
    perguntar(pergunta, destrutivo, (confirmou) => {
      if (!confirmou) return;
      confirmados.add(form);
      try {
        form.requestSubmit(submitter ?? undefined);
      } finally {
        confirmados.delete(form);
      }
    });
  }, true);

  /* ---------- Bloqueio de duplo envio ---------- */
  const botoesEnvio = (form) => [...form.querySelectorAll('button:not([type="button"]):not([type="reset"]), input[type="submit"]')];

  const restaurarBotoes = (form) => {
    botoesEnvio(form).forEach((botao) => {
      if (botao.dataset.textoOriginal !== undefined) {
        if (botao.tagName === "INPUT") botao.value = botao.dataset.textoOriginal;
        else botao.innerHTML = botao.dataset.textoOriginal;
        delete botao.dataset.textoOriginal;
      }
      if (botao.dataset.bloqueadoPorEnvio) {
        botao.disabled = false;
        botao.removeAttribute("aria-busy");
        delete botao.dataset.bloqueadoPorEnvio;
      }
    });
    form.classList.remove("is-submitting");
  };

  document.addEventListener("submit", (event) => {
    const form = event.target;
    if (!(form instanceof HTMLFormElement) || event.defaultPrevented) return;
    if ((form.method || "get").toLowerCase() !== "post" || form.hasAttribute("data-no-lock") || form.target === "_blank") return;

    const submitter = event.submitter;
    // Só depois que o navegador montou os dados do formulário: botão desabilitado não envia name/value.
    window.setTimeout(() => {
      form.classList.add("is-submitting");
      botoesEnvio(form).forEach((botao) => {
        botao.dataset.bloqueadoPorEnvio = "1";
        botao.disabled = true;
      });
      const alvo = submitter && form.contains(submitter) ? submitter : botoesEnvio(form)[0];
      if (alvo) {
        alvo.setAttribute("aria-busy", "true");
        if (alvo.tagName === "INPUT") { alvo.dataset.textoOriginal = alvo.value; alvo.value = "Enviando…"; }
        else { alvo.dataset.textoOriginal = alvo.innerHTML; alvo.textContent = "Enviando…"; }
      }
      // Respostas que baixam arquivo não recarregam a página: libera o botão depois de um tempo.
      window.setTimeout(() => restaurarBotoes(form), 20000);
    }, 0);
  });

  window.addEventListener("pageshow", (event) => {
    if (event.persisted) document.querySelectorAll("form.is-submitting").forEach(restaurarBotoes);
  });

  /* ---------- Linhas de tabela clicáveis (data-href) ---------- */
  const interativo = "a, button, input, select, textarea, label, summary, details, [role='button'], [data-no-row-click]";

  document.addEventListener("click", (event) => {
    const linha = event.target instanceof Element ? event.target.closest("tr[data-href]") : null;
    if (!linha || event.defaultPrevented || event.button !== 0) return;
    if (event.target.closest(interativo)) return;
    if (String(window.getSelection?.() ?? "").length > 0) return;
    const destino = linha.getAttribute("data-href");
    if (!destino) return;
    if (event.ctrlKey || event.metaKey || event.shiftKey) window.open(destino, "_blank", "noopener");
    else window.location.assign(destino);
  });

  /* ---------- Botão "Voltar" (data-history-back) ---------- */
  document.addEventListener("click", (event) => {
    const voltar = event.target instanceof Element ? event.target.closest("[data-history-back]") : null;
    if (!voltar || event.defaultPrevented) return;
    if (window.history.length > 1 && document.referrer.startsWith(window.location.origin)) {
      event.preventDefault();
      window.history.back();
    }
  });
})();

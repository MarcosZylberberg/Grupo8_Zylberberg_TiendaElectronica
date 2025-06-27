  document.getElementById('reservaForm').addEventListener('submit', function(e) {
    e.preventDefault();

    const nombre   = document.getElementById('nombre').value.trim();
    const telefono = document.getElementById('telefono').value.trim();
    const fecha    = document.getElementById('fecha').value;
    const hora     = document.getElementById('hora').value;
    const personas = document.getElementById('personas').value;

    console.log('validaciones.js cargado');
    
    if (!nombre) {
      alert('Por favor ingresa tu nombre.');
      return;
    }

    if (telefono.length < 8 || isNaN(telefono)) {
      alert('Teléfono inválido. Mínimo 8 dígitos numéricos.');
      return;
    }

    if (!fecha) {
      alert('Selecciona una fecha.');
      return;
    }

    if (!hora) {
      alert('Selecciona una hora.');
      return;
    }

    if (personas < 1) {
      alert('La cantidad de personas debe ser al menos 1.');
      return;
    }

    this.submit();
  });
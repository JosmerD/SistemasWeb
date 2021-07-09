
class Cursos extends Uploadpicture {
    constructor() {
        super();
        this.Image = null;
        this.CursoID = 0;
    }
    RegistrarCurso() {

        var data = new FormData();
        data.append('Input.Curso', $("#curNombre").val());
        data.append('Input.Informacion', $("#curDescripcion").val());
        data.append('Input.Horas', $("#curHoras").val());
        data.append('Input.Costo', $("#curCosto").val());
        data.append('Input.Estado', document.getElementById("curEstado").checked);
        data.append('Input.CategoriaID', $("#curCategoria").val());
        data.append('Input.Image', this.Image);
        data.append('Input.CursoID', $("#curCursoID").val());

        $.each($('input[type=file]')[0].files, (i, file) => {
            data.append('AvatarImage', file);
        });
        $.ajax({
            url: "GetCurso",
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            type: 'POST',
            success: (result) => {
                try {
                    var item = JSON.parse(result);
                    if (item.Code == "Done") {
                        window.location.href = "Cursos";
                    } else {
                        document.getElementById("mensaje").innerHTML = item.Description;
                    }
                } catch (e) {
                    document.getElementById("mensaje").innerHTML = result;
                }
                console.log(result);
            }
        });

    }
    EditCurso(curso, cat) {
        let j = 1;
        $("#curNombre").val(curso.Curso);
        $("#curDescripcion").val(curso.Informacion);
        $("#curHoras").val(curso.Horas);
        $("#curCosto").val(curso.Costo);
        $("#curEstado").prop("checked", curso.Estado);
        $("#curCursoID").val(curso.CursoID);
        this.Image = curso.Image;

        document.getElementById("cursoImage").innerHTML = "<img class='cursoImage' src='data:image/jpeg;base64," + curso.Image + "' />";
        let x = document.getElementById("curCategoria");
        x.options.length = 0;
        for (var i = 0; i < cat.length; i++) {
            if (cat[i].Value == curso.CategoriaID) {

                x.options[0] = new Option(cat[i].Text, cat[i].Value);
                x.selectedIndex = 0;
                j = i;
            } else {

                x.options[i] = new Option(cat[i].Text, cat[i].Value);
            }

        }
        x.options[j] = new Option(cat[0].Text, cat[0].Value);

    }
    EliminarCurso() {
        $.post(
            "EliminarCurso",
            { CursoID: this.CursoID },
            (response) => {
                var item = JSON.parse(response);
                if (item.Code == "Done") {
                    window.location.href = "Cursos";
                } else {
                    document.getElementById("mensajeEliminar").innerHTML = item.Description;
                }
            }
        );
    }
    GetCurso(data) {
        document.getElementById("titleCurso").value = data.Nombre;
        this.CursoID = data.CursoID;
    }
    Restablecer() {
        $("#curNombre").val("");
        $("#curDescripcion").val("");
        $("#curHoras").val("");
        $("#curCosto").val("");
        $("#curEstado").prop("checked", false);
        $("#curCursoID").val("");

        document.getElementById("cursoImage").innerHTML = ['<img class="cursoImage" src="',"/Images/logo-google.png",' "/>'].join('');
       
    }
}
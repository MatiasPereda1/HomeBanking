var app = new Vue({
    el:"#app",
    data:{
        clientInfo: {},
        //error: null
        errorToats: null,
        errorMsg: null,
    },
    methods:{
        getData: function(){
            //axios.get("/api/clients/1")
            let token = sessionStorage.getItem('TOKEN');
            axios.get("/api/clients/current",
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            )
            .then(function (response) {
                //get client ifo
                app.clientInfo = response.data;
            })
            .catch(function (error) {
                // handle error
                //app.error = error;
                this.errorMsg = "Error getting data";
                this.errorToats.show();
            })
        },
        formatDate: function(date){
            return new Date(date).toLocaleDateString('en-gb');
        },
        signOut: function () {
            sessionStorage.clear();
            window.location.href = "/index.html";
        },
        create: function () {
            let token = sessionStorage.getItem('TOKEN');
            axios.post('/api/clients/current/accounts', null,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            )
            .then(response => window.location.reload())
            .catch((error) =>{
                this.errorMsg = error.response.data;  
                this.errorToats.show();
            })
        }        
    },
    mounted: function () {
        this.errorToats = new bootstrap.Toast(document.getElementById('danger-toast'));
        this.getData();
    }
})
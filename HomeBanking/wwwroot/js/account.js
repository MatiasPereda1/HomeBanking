var app = new Vue({
    el:"#app",
    data:{
        accountInfo: {},
        //error: null
        errorToats: null,
        errorMsg: null,
    },
    methods:{
        getData: function () {
            let token = sessionStorage.getItem('TOKEN');
            const urlParams = new URLSearchParams(window.location.search);
            const id = urlParams.get('id');
            axios.get(`/api/accounts/${id}`,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            )
            .then(function (response) {
                //get client ifo
                app.accountInfo = response.data;
                app.accountInfo.transactions.sort((a,b) => parseInt(b.id - a.id))
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
    },
    mounted: function () {
        this.errorToats = new bootstrap.Toast(document.getElementById('danger-toast'));
        this.getData();
    }
})
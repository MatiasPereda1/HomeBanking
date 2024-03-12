var app = new Vue({
    el: "#app",
    data: {
        clientInfo: {},
        error: null,
        creditCards: [],
        debitCards: [],
        errorToats: null,
        errorMsg: null,
    },
    methods: {
        getData: function () {
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
                    app.creditCards = app.clientInfo.cards.filter(card => card.type == "CREDIT");
                    app.debitCards = app.clientInfo.cards.filter(card => card.type == "DEBIT");
                })
                .catch(function (error) {
                    // handle error
                    this.errorMsg = "Error getting data";
                    this.errorToats.show();
                })
        },
        formatDate: function (date) {
            return new Date(date).toLocaleDateString('en-gb');
        },
        signOut: function(){
            sessionStorage.clear();
            window.location.href = "/index.html";
        },
    },
    mounted: function () {
        this.errorToats = new bootstrap.Toast(document.getElementById('danger-toast'));
        this.getData();
    }
})
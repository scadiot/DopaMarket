<template>
  <div>
    Items
  </div>
</template>

<script>
import axios from 'axios';

export default {
    created() {
        axios.get('http://localhost:60037/Api/Item/GetAllItems')
        .then(function (response) {
            // handle success
            console.log(response);
        })
        .catch(function (error) {
            // handle error
            console.log(error);
        })
        .finally(function () {
            // always executed
            return;
        });
    }
}
</script>

<style>

</style>
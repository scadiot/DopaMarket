<template>
  <div id="app">
    <img alt="Vue logo" src="./assets/logo.png">
    <ItemEditor />
  </div>
</template>

<script>
import ItemEditor from './components/ItemEditor/Main.vue'

export default {
  name: 'app',
  components: {
    ItemEditor
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>

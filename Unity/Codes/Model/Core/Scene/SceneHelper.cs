namespace ET {
    public static class SceneHelper {
// 会感觉这些：对于服务器端的很多概念，还没有特别清楚的认识， Domain

        public static int DomainZone(this Entity entity) {
            return ((Scene) entity.Domain)?.Zone ?? 0;
        }

        public static Scene DomainScene(this Entity entity) {
            return (Scene) entity.Domain;
        }
    }
}
<template>
  <div>
    <el-breadcrumb separator-class="el-icon-arrow-right">
      <el-breadcrumb-item :to="{ path: '/home' }">宠物主人</el-breadcrumb-item>
      <el-breadcrumb-item>我的</el-breadcrumb-item>
      <el-breadcrumb-item>申请管理</el-breadcrumb-item>
    </el-breadcrumb>

    <el-card class="workCard" v-loading="loading">
      <el-row class="header">
        <el-col class="titlecol" :span="5">
          <p class="title">申请管理</p>
        </el-col>
      </el-row>
      <!--搜索与添加-->
      <el-row :gutter="30">
        <el-col :span="7">
          <el-input placeholder="请输入喂养师名称" v-model="queryInfo.query" clearable @clear="getWorklist()">
            <el-button
              slot="append"
              icon="el-icon-search"
              @click="getWorklist()"
            ></el-button>
          </el-input>
        </el-col>
        <el-col :span="2">
          <el-button
            type="success"
            icon="el-icon-plus"
            @click="$router.push('/Recruitment')"
            >我要申请</el-button
          >
        </el-col>
      </el-row>
      <!--表格区域-->
      <el-row style="margin-top:1cm">
        <el-table :data="Worklist" border>
          <el-table-column type="index" label="#"> </el-table-column>
          <el-table-column prop="work_name" label="喂养师名称"> </el-table-column>
          <el-table-column prop="address" label="期望工作地址"> </el-table-column>
          <el-table-column prop="salary" label="期望工作薪水"> </el-table-column>
          <el-table-column prop="total_time" label="期望工作时长">
          </el-table-column>
          <el-table-column label="工作状态" width="80">
            <template slot-scope="scope">
              <el-tag type="success" v-if="scope.row.status == 0"
                >申请中</el-tag
              >
              <el-tag type="danger" v-if="scope.row.status == 1">已同意</el-tag>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="200">
            <template slot-scope="scope">
              
              
              <el-button
                type="danger"
                size="medium"
                icon="el-icon-circle-close"
                :disabled="!scope.row.status"
                @click="
                  deleteVisible = true;
                  work_id = scope.row.work_id;
                "
                >删除工作</el-button
              >
            </template>
          </el-table-column>
          <el-table-column type="expand" label="详情" width="75">
            <template slot-scope="scope">
              <el-row>
                <el-col :span="12">
                  <el-image :src="scope.row.cover" fit="fit">
                    <div slot="error" class="image-slot">
                      <i
                        class="el-icon-picture-outline"
                        style="font-size: 100px"
                      ></i>
                    </div>
                  </el-image>
                </el-col>
                <el-col
                  :span="8"
                  class="inlineCol"
                  style="text-align:left"
                  :offset="2"
                >
                  <p class="inlineTitle">
                    <i class="el-icon-paperclip"></i> 工作名称 ：
                    <span class="inlineInfo"> {{ scope.row.work_name }} </span>
                  </p>
                  <p class="inlineTitle">
                    <i class="el-icon-tickets "></i> 工作描述 ：
                    <span class="inlineInfo">
                      {{ scope.row.work_description }}
                    </span>
                  </p>
                  <p class="inlineTitle">
                    <i class="el-icon-thumb"></i> 点赞 ：
                    <span class="inlineInfo"> {{ scope.row.likes_num }} </span>
                  </p>
                  <p class="inlineTitle">
                    <i class="el-icon-star-off"></i> 收藏 ：
                    <span class="inlineInfo">
                      {{ scope.row.collect_num }}
                    </span>
                  </p>
                  <p class="inlineTitle">
                    <i class="el-icon-date"></i> 工作周期 ：
                    <span class="inlineInfo"> {{ scope.row.start_day }} </span>
                    ~ <span class="inlineInfo"> {{ scope.row.end_day }} </span>
                  </p>
                  <p class="inlineTitle">
                    <i class="el-icon-time"></i> 工作时间 ：
                    <span class="inlineInfo" >{{week_list[scope.row.week_day]}}</span>
                    <span class="inlineInfo"> {{ scope.row.start_time }} </span>
                    ~ <span class="inlineInfo"> {{ scope.row.end_time }} </span>
                  </p>
                </el-col>
              </el-row>
            </template>
          </el-table-column>
        </el-table>
      </el-row>
      <!--分页区域-->
      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="queryInfo.pagenum"
        :page-sizes="[5, 10, 15]"
        :page-size="queryInfo.pagesize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="total"
        style="margin-top:2cm"
      >
      </el-pagination>
    </el-card>

    

    

    <!--删除工作对话框-->
    <el-dialog title="提示" :visible.sync="deleteVisible" width="30%" center>
      <div style="text-align:center">
        <span>删除申请后该申请将不会出现在你的申请列表里，您确定吗?</span>
      </div>
      <span slot="footer" class="dialog-footer">
        <el-button @click="deleteVisible = false">取 消</el-button>
        <el-button
          type="primary"
          @click="
            handleDelete();
            deleteVisible = false;
          "
          >确 定</el-button
        >
      </span>
    </el-dialog>
  </div>
</template>

<script>
import axios from "axios";
export default {
  data() {
    return {
      week_list:['未设置','星期一','星期二','星期三','星期四','星期五','星期六','星期日'],
      loading: true,
      Worklist: [],
      queryInfo: {
        pagenum: 1,
        pagesize: 5,
        query: "",
      },
      total: 20,
      AbsentVisible: false,

      AbForm: {
        work_id: "",
        content: "",
        proof: "",
        leave_day: "",
        leave_start: "",
        leave_end: "",
      },

      AbFormRules: {
        leave_day: [{ required: true, message: "请选择时间" }],
        content: [{ required: true, message: "请输入请假理由" }],
        leave_start: [{ required: true, message: "请选择请假的起始时间" }],
        leave_end: [{ required: true, message: "请选择请假的结束时间" }],
      },

      resignVisible: false,
      work_id: 0,
      deleteVisible: false,
      workstart: "",
      workend: "",
      startTime: "",
      endTime: "",
      workDay: "",

      pickerOptions0: {
        //设置日期必须在工作时间之间
        disabledDate: (time) => {
          if (this.workstart)
            return (
              new Date(this.workstart).getTime() > time.getTime() ||
              new Date(this.workend).getTime() < time.getTime() ||
              time.getDay() != this.workDay % 7
            );
        },
      },
    };
  },
  created() {
    this.getWorklist();
  },
  methods: {
    async getWorklist() {
      const { data: res } = await axios.post(
        this.$helper.endpointUrl("/Work/FindOwnWork"),
        this.queryInfo,
        { withCredentials: true }
      );
      this.Worklist = res.worklist;
      this.total = res.totalpage;
      this.loading = false;
    },
    handleSizeChange(newSize) {
      this.queryInfo.pagesize = newSize;
      this.getWorklist();
    },
    handleCurrentChange(newPage) {
      this.queryInfo.pagenum = newPage;
      this.getWorklist();
    },
    AbFormClose() {
      this.$refs.AbFormRef.resetFields();
    },
    addAbsentPre() {
      this.$refs.AbFormRef.validate(async (valid) => {
        if (!valid) {
          this.$message.error("请按照验证提示正确填写请假申请后再进行提交");
          return;
        }
        if (this.AbForm.proof == "") {
          this.AbForm.proof = "无备注";
        }
        const { data: res } = await axios.post(
          this.$helper.endpointUrl("/Leave/RequestRest"),
          this.AbForm,
          { withCredentials: true }
        );
        if (res == -1) {
          this.$message.error("请假申请提交失败，请稍后重试");
          return;
        }
        if (res == -21 || res == -22) {
          this.$message.error("抱歉，您不能在同一天重复申请请假");
          return;
        }
        this.$message.success("请假申请提交成功,您可以在请假管理中进行查看");
        this.AbsentVisible = false;
      });
    },
    async handleResign() {
      const { data: res } = await axios.post(
        this.$helper.endpointUrl("/Work/Getresign"),
        { work_id: this.work_id },
        { withCredentials: true }
      );
      this.getWorklist();
      if (res == 1) {
        this.$message.success(
          "您辞职的消息已经发给工作发布者，感谢您参与工作！"
        );
      } else {
        this.$message.error("辞职申请发送失败，请稍后重试");
      }
    },
    async handleDelete() {
      const { data: res } = await axios.post(
        this.$helper.endpointUrl("/Work/Deleteresign"),
        { work_id: this.work_id },
        { withCredentials: true }
      );
      this.getWorklist();
      if (res == 1) {
        this.$message.success("该申请已经成功地从申请管理中移除");
      } else {
        this.$message.error("删除操作执行失败，请稍后重试");
      }
    },
  },
};
</script>

<style scoped>
.workCard {
  margin-top: 20px;
  padding: 0;
}
.title {
  font-size: 25px;
  font-weight: bold;
}
.titlecol {
  margin-bottom: 1cm;
}
.inlineCol {
  text-align: center;
}
.inlineTitle {
  font-weight: bold;
  color: grey;
  margin-top: 1cm;
}
.inlineInfo {
  font-weight: normal;
  color: black;
}
</style>
